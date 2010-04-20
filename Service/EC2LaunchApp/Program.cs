using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon;

namespace EC2LaunchApp
{
	class Program
	{
		private static string _awsKey;
		private static string _awsSecretKey;
		private static string _instanceId;
		private static int _stopMinutes;
		private static int _launchMinutes;

		static void Main(string[] args)
		{
			_awsKey = ConfigurationManager.AppSettings["awsKey"];
			_awsSecretKey = ConfigurationManager.AppSettings["awsSecret"];
			_instanceId = ConfigurationManager.AppSettings["instanceId"];
			_stopMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["stop-minutes"]);
			_launchMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["launch-minutes"]);

			if (args[0] == "launch")
				LaunchInstance();
			else if (args[0] == "stop")
				StopInstance();
		}

		private static void LaunchInstance()
		{
			try
			{
				AmazonEC2 ec2 = AWSClientFactory.CreateAmazonEC2Client(_awsKey, _awsSecretKey);

				StartInstancesRequest request = new StartInstancesRequest();
				request.WithInstanceId(new string[] { _instanceId });

				ec2.StartInstances(request);
				Mail(string.Format("Successfully started EC2 instance {0}", _instanceId));
			}
			catch (Exception e)
			{
				MailError("Error launching instance", e);
			}
		}

		private static void StopInstance()
		{
			try
			{
				AmazonEC2 ec2 = AWSClientFactory.CreateAmazonEC2Client(_awsKey, _awsSecretKey);

				StopInstancesRequest request = new StopInstancesRequest();
				request.WithInstanceId(new string[] { _instanceId });

				ec2.StopInstances(request);
				Mail(string.Format("Successfully stopped EC2 instance {0}", _instanceId));
			}
			catch (Exception e)
			{
				MailError("Error stopping instance", e);
			}	
		}

		private static void Mail(string subject)
		{
			try
			{
				string to = "support@rlysimple.com";
				string from = "support@rlysimple.com";
				string body = string.Format("{0} - {1}",DateTime.UtcNow,subject);

				SmtpClient client = new SmtpClient("localhost", 25);
				MailMessage message = new MailMessage(from, to, subject, body);
				message.IsBodyHtml = true;
				message.BodyEncoding = Encoding.UTF8;
				client.Send(message);
			}
			catch (Exception)
			{
			}
		}

		private static void MailError(string subject,Exception e)
		{
			try
			{
				string to = "support@rlysimple.com";
				string from = "support@rlysimple.com";
				string body = e.Message + "\n";
				body += e.ToString();

				SmtpClient client = new SmtpClient("localhost", 25);
				MailMessage message = new MailMessage(from, to, subject, body);
				message.IsBodyHtml = true;
				message.BodyEncoding = Encoding.UTF8;
				client.Send(message);
			}
			catch (Exception)
			{
			}
		}
	}
}
