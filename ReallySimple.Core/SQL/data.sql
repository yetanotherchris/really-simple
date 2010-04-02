-- `categories`
INSERT INTO categories (id, title) VALUES ('3FF1CB03-2758-4965-AB7E-B10E9136FBA2', 'Environment');
INSERT INTO categories (id, title) VALUES ('C6C0036B-C991-4136-9580-4ED57ED97FB9', 'Film');
INSERT INTO categories (id, title) VALUES ('C19B429C-3933-4A7C-B986-F7C16633A9DB', 'Finance');
INSERT INTO categories (id, title) VALUES ('4395EAB2-1B91-4BB7-AF0D-A9A4FBE9A4A9', 'Gaming');
INSERT INTO categories (id, title) VALUES ('C853CFE7-526D-4029-841E-374A4494D3F5', 'Music');
INSERT INTO categories (id, title) VALUES ('AEA6435A-9271-42D1-906E-3DE43FDB69EB', 'News');
INSERT INTO categories (id, title) VALUES ('C46A37AB-AAA7-4DD1-968F-A7E8AEAA3EA1', 'Science');
INSERT INTO categories (id, title) VALUES ('798BDC11-5001-4FFC-9125-13013104B23E', 'Sport');
INSERT INTO categories (id, title) VALUES ('D7E484AC-BFF5-4D40-882E-6E935B9ACE05', 'Technology');
INSERT INTO categories (id, title) VALUES ('0F385B3E-ABC0-4C73-8FE2-0C46E6133E6E', 'Travel');

-- `feeds`
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('3EB122CB-7F96-45FD-A882-99ACEDCBA943', '3FF1CB03-2758-4965-AB7E-B10E9136FBA2', '85812F48-67CE-4B87-88BD-B6A5D1B15375', 'http://feeds.feedburner.com/cleantechnica/com/', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('10E48299-17D7-4EE6-A924-FCC7D004A838', '3FF1CB03-2758-4965-AB7E-B10E9136FBA2', 'FB0FD745-B11C-406C-BC24-B6E9AA96FFE7', 'http://www.ecofriend.org/rss.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('80A68D8D-88C3-490C-9BFA-0A89CAE7E83F', '3FF1CB03-2758-4965-AB7E-B10E9136FBA2', '650DE5A6-F53E-4BD8-AF77-1A3554ABDC2E', 'http://www.ecogeek.org/index.php?format=feed&type=rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('DBD2CEEC-EDFF-40D4-97D2-34F8FC3831DD', '3FF1CB03-2758-4965-AB7E-B10E9136FBA2', 'B0680A9F-048E-47F1-83DA-2A8D4CB0576B', 'http://feeds.feedburner.com/Inhabitat', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('2C224F6B-E6FB-4F6E-B4E5-1034C14640A1', '3FF1CB03-2758-4965-AB7E-B10E9136FBA2', 'C0253B4F-4DB4-44BA-9492-D955F2D5ABA8', 'http://feeds.feedburner.com/treehugger/science-technology', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('B9C57B94-DE77-436C-BD4C-9D33434C35EF', 'C6C0036B-C991-4136-9580-4ED57ED97FB9', '6C914EEA-98C5-4F42-8397-43BFE4669635', 'http://rss.feedsportal.com/c/592/f/7507/index.rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('5A263C6E-8C6A-403C-8DA1-A597F9A055C1', 'C6C0036B-C991-4136-9580-4ED57ED97FB9', '3DE7EA64-9DB0-4D68-989A-74302FD84A62', 'http://i.rottentomatoes.com/syndication/rss/top_news.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('2B634E52-CD8F-4872-B211-D3F78E50F1EE', 'C6C0036B-C991-4136-9580-4ED57ED97FB9', '6BE67089-19FC-45DC-9588-6141C3119039', 'http://feeds2.feedburner.com/slashfilm', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('F4BB7C23-757A-4BD8-A58E-67BFEF513DB6', 'C6C0036B-C991-4136-9580-4ED57ED97FB9', '76ACBEF0-0070-4182-90A7-47B8BFC44A6F', 'http://feeds.feedburner.com/totalfilm/news', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('3A40379D-A93B-411A-9310-E78E65AF7608', 'C6C0036B-C991-4136-9580-4ED57ED97FB9', 'F96A35FD-1BED-42ED-8A29-E907E4FE8CF4', 'http://rss.ent.yahoo.com/movies/movie_headlines.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('69D4444B-E749-41F7-BEC3-3377427501B7', 'C19B429C-3933-4A7C-B986-F7C16633A9DB', '80EBBC2F-8177-4643-88A8-EC4F5A5028FB', 'http://hosted.ap.org/lineups/BUSINESSHEADS-rss_2.0.xml?SITE=FLPEJ&SECTION=HOME', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('4963E75D-EA9D-41B3-835C-04F15C9359E5', 'C19B429C-3933-4A7C-B986-F7C16633A9DB', '0E01F866-1039-46F2-8483-9D7B3D13E95A', 'http://rss.cnn.com/rss/money_topstories.rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('E90CF4EE-B18A-4DAF-846D-8E6A9D73D6E4', 'C19B429C-3933-4A7C-B986-F7C16633A9DB', '1AA696F6-2805-4C4F-BB04-2FA82E7D27C6', 'http://www.ft.com/rss/markets', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('09DAF17D-7F97-4CA0-8E8C-59CC76915ED8', 'C19B429C-3933-4A7C-B986-F7C16633A9DB', '0B2A190E-BE4F-401D-8FAF-7E09D49E3AFA', 'http://feeds.marketwatch.com/marketwatch/topstories/', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('8F09D8D7-9D17-4987-8A94-E33479B327BE', 'C19B429C-3933-4A7C-B986-F7C16633A9DB', '1FB6C99D-1D16-400E-9C72-1F886CFED693', 'http://feeds.reuters.com/reuters/businessNews', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('4EC41E09-CFC3-452B-8422-4C4602D661D8', '4395EAB2-1B91-4BB7-AF0D-A9A4FBE9A4A9', '78E794B0-F3AF-4A29-B892-36B48DCEBA6B', 'http://www.computerandvideogames.com/rss/feed.php?limit=30', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('E10F63C0-6C5D-4867-801D-3E5C8604526E', '4395EAB2-1B91-4BB7-AF0D-A9A4FBE9A4A9', 'A3DD9C7D-C626-4758-8F01-606500468A72', 'http://www.destructoid.com/elephant/index.phtml?mode=atom', 2, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('B3A8C812-4B33-4C71-8E91-265E7C371233', '4395EAB2-1B91-4BB7-AF0D-A9A4FBE9A4A9', 'D627DB03-E603-47FD-AC1A-54C7466122D2', 'http://kotaku.com/tag/top/index.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('DBBF1B37-5F20-4E41-9A65-55B511867935', '4395EAB2-1B91-4BB7-AF0D-A9A4FBE9A4A9', '669C3B0A-44CC-4D92-BFC6-10CC85B64155', 'http://feeds.feedburner.com/Massively', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('C3763785-8C38-44A9-B92E-E31E26C6FEF0', '4395EAB2-1B91-4BB7-AF0D-A9A4FBE9A4A9', '6445C27A-CC93-4770-8F66-92295EEDB721', 'http://feed.shacknews.com/shackfeed.xml', 1, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('763DA835-1085-40C0-ADB1-A713928A4642', 'C853CFE7-526D-4029-841E-374A4494D3F5', 'DC01AB83-58AA-4115-B5E7-64A0971AD0B6', 'http://music-mix.ew.com/feed/', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('CC6067EC-A74C-4CF7-8B77-6942BA2E45D8', 'C853CFE7-526D-4029-841E-374A4494D3F5', '7BCBE1BB-9E72-4A4A-96D6-CBEEA5D2D42F', 'http://feeds.eonline.com/eonline/musicnews', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('B38C73D5-EEF0-4552-AB24-A0B1F16197F6', 'C853CFE7-526D-4029-841E-374A4494D3F5', '7435A9B5-BB2B-494C-8707-7DED42F73CC6', 'http://music.aol.com/api/v3/news/getAllMainNews?f=rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('3EC2EFA5-C2ED-4838-A9AB-92C65E45ED29', 'C853CFE7-526D-4029-841E-374A4494D3F5', '61C01117-CCDD-4519-AAA5-36218EAC6D82', 'http://www.mtv.com/rss/news/news_full.jhtml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('04C7112C-9E31-4275-A233-81CA07CDE605', 'C853CFE7-526D-4029-841E-374A4494D3F5', '6B4BCA53-F934-422C-987C-0D336B86AB98', 'http://www.popeater.com/category/music-news/rss.xml', 1, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('94F4A70F-BFB4-49E0-BA97-D96C660195C2', 'AEA6435A-9271-42D1-906E-3DE43FDB69EB', '80EBBC2F-8177-4643-88A8-EC4F5A5028FB', 'http://hosted.ap.org/lineups/TOPHEADS-rss_2.0.xml?SITE=NJMOR&SECTION=HOME', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('0AB96DC2-6DA4-44AD-BDA5-FE284584103A', 'AEA6435A-9271-42D1-906E-3DE43FDB69EB', 'F96A35FD-1BED-42ED-8A29-E907E4FE8CF4', 'http://feeds.timesonline.co.uk/c/32313/f/440158/index.rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('84DF065A-D5FE-46D4-A785-94E0389B1FC6', 'AEA6435A-9271-42D1-906E-3DE43FDB69EB', '97A6E038-5970-4E09-8E60-64C57F8D0D1D', 'http://pheedo.msn.com/id/3032091/device/rss/io', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('34985E7E-F601-43D7-86D5-BCC233747469', 'AEA6435A-9271-42D1-906E-3DE43FDB69EB', '07F60827-BDBE-49C1-A9BC-53CCBB6037AC', 'http://news.google.co.uk/news?pz=1&cf=all&ned=uk&hl=en&topic=w&output=rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('EEA61E4C-ADF0-4A4C-B24E-71D0DF02BEF2', 'AEA6435A-9271-42D1-906E-3DE43FDB69EB', '1FB6C99D-1D16-400E-9C72-1F886CFED693', 'http://feeds.reuters.com/reuters/topNews', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('6E4A8672-24BD-4905-9D65-B9BF223FBE18', 'C46A37AB-AAA7-4DD1-968F-A7E8AEAA3EA1', 'B7B3BF80-1C1E-4642-B519-FCA0DBEA7E0C', 'http://www.bbcfocusmagazine.com/rss/all/rss.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('BF0A0742-DA84-4859-B310-A62BF0559FD7', 'C46A37AB-AAA7-4DD1-968F-A7E8AEAA3EA1', '66EAEBA5-B17B-4FF6-A328-ED74A7DB0DEB', 'http://www.newscientist.com/blogs/shortsharpscience/atom.xml', 2, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('CA3CA436-3C77-4343-AC30-6333E70D0C79', 'C46A37AB-AAA7-4DD1-968F-A7E8AEAA3EA1', 'A00CE5DA-AF68-4FDA-95A8-6AAA3C0B24C5', 'http://www.popsci.com/rss.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('0DA62AAE-936A-4345-93A1-248CCC73DCBF', 'C46A37AB-AAA7-4DD1-968F-A7E8AEAA3EA1', '2FE57DC6-407B-45DB-ACE7-3E71455807AD', 'http://feeds.sciencedaily.com/sciencedaily', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('AE9EA6B4-4639-412F-A5BC-3D5B2616DFDA', 'C46A37AB-AAA7-4DD1-968F-A7E8AEAA3EA1', '64A11041-C45F-486D-AF32-6C66BA4BC25A', 'http://rss.sciam.com/ScientificAmerican-News', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('6222F488-CD44-4143-B226-30F627FA7657', '798BDC11-5001-4FFC-9125-13013104B23E', '3F8FF9D1-ADAE-48D8-B02C-DA7032622AB2', 'http://feeds.digg.com/digg/container/sports/popular.rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('74CE8262-8501-485A-AADF-4392F9C93272', '798BDC11-5001-4FFC-9125-13013104B23E', '6B518004-19A9-43B4-8DF0-495DDF78988C', 'http://eurosport.yahoo.com/eurosport/tickerdb/sport/0.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('06E4A1AE-6A03-4011-AF54-E810E2220F88', '798BDC11-5001-4FFC-9125-13013104B23E', '955C1A29-FB79-44F6-8E89-FDBC4EEB64BE', 'http://www.skysports.com/rss/0,20514,12040,00.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('F736AFB6-B8BC-4F1F-86CE-19695F92B4BC', '798BDC11-5001-4FFC-9125-13013104B23E', '955C1A29-FB79-44F6-8E89-FDBC4EEB64BE', 'http://www.skysports.com/rss/0,20514,11661,00.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('97BF991E-E3E3-480E-850E-DF81FB739D64', '798BDC11-5001-4FFC-9125-13013104B23E', '8A4CE7F4-FDE7-497A-8D04-96B6062A1705', 'http://rss.cnn.com/rss/si_latest.rss', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('E0E63B9D-7B0B-4028-A62C-5CE187999497', 'D7E484AC-BFF5-4D40-882E-6E935B9ACE05', '0679E8C0-FECD-4175-A96D-3D7C6BD06653', 'http://feeds.arstechnica.com/arstechnica/index', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('6E7194DB-2C7F-4561-A03E-31BD76B14390', 'D7E484AC-BFF5-4D40-882E-6E935B9ACE05', '78FB160C-97FE-46C1-9532-12FBC29D65AA', 'http://www.engadget.com/rss.xml', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('3D995A3A-B418-48BC-AE72-4986F3909264', 'D7E484AC-BFF5-4D40-882E-6E935B9ACE05', '2C3EDA7B-CC93-457C-BDD7-782AC615A9E0', 'http://rss.slashdot.org/Slashdot/classic', 1, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('929F4EB3-F471-49B8-9324-4D3B264B1BD2', 'D7E484AC-BFF5-4D40-882E-6E935B9ACE05', '3A3FCEA3-31CF-49A0-A685-52032319627F', 'http://www.techeblog.com/elephant/?mode=atom', 2, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('CD63988E-AC17-4EDB-9DBD-437946AE1341', 'D7E484AC-BFF5-4D40-882E-6E935B9ACE05', '8FB41C88-0ED3-4E9F-A0DC-C2595507247B', 'http://feeds.feedburner.com/yankodesign', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('52831EF0-C453-4EE0-A9C1-35DD4EDA4031', '0F385B3E-ABC0-4C73-8FE2-0C46E6133E6E', '88A3E341-CD3D-4053-8352-8C4C58945840', 'http://feeds.concierge.com/ConciergeTravelFeatures', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('F1463FD4-0F41-4F3B-AF0C-D3574B98FA31', '0F385B3E-ABC0-4C73-8FE2-0C46E6133E6E', '0A300B6D-64EC-403A-BE1E-047E40FB6EFA', 'http://feeds2.feedburner.com/fodors/travel-news', 1, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('476DA1F2-FC9D-4245-8A01-7E64F670C33D', '0F385B3E-ABC0-4C73-8FE2-0C46E6133E6E', 'D78C71F0-CBE7-4846-9930-485E574B259A', 'http://inside-digital.blog.lonelyplanet.com/feed/', 0, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('39A03C3F-7EB8-4D99-A78A-B172D033CCD7', '0F385B3E-ABC0-4C73-8FE2-0C46E6133E6E', '6D799E67-D7A0-41EA-BBD1-8FCFD88827DE', 'http://feeds.nationalgeographic.com/ng/TRAVELERBlogs/Intelligent_Travel', 2, NULL, '');
INSERT INTO feeds (id, categoryid, siteid, url, [type], lastupdate, cleaner) VALUES ('04E983F6-9EA1-496F-9E60-F0C234D68A09', '0F385B3E-ABC0-4C73-8FE2-0C46E6133E6E', '96F4F818-3F1E-42CE-B88D-5E4E62F1B490', 'http://www.travelandleisure.com/blogs/carry-on?format=atom', 2, NULL, '');

-- `sites`
INSERT INTO sites (id, url, title) VALUES ('85812F48-67CE-4B87-88BD-B6A5D1B15375', 'http://www.cleantechnica.com', 'Cleantechnica');
INSERT INTO sites (id, url, title) VALUES ('FB0FD745-B11C-406C-BC24-B6E9AA96FFE7', 'http://www.ecofriend.org', 'Ecofriend');
INSERT INTO sites (id, url, title) VALUES ('650DE5A6-F53E-4BD8-AF77-1A3554ABDC2E', 'http://www.ecogeek.org', 'Ecogeek');
INSERT INTO sites (id, url, title) VALUES ('B0680A9F-048E-47F1-83DA-2A8D4CB0576B', 'http://www.inhabitat.com', 'Inhabitat');
INSERT INTO sites (id, url, title) VALUES ('C0253B4F-4DB4-44BA-9492-D955F2D5ABA8', 'http://www.treehugger.com', 'Treehugger');
INSERT INTO sites (id, url, title) VALUES ('6C914EEA-98C5-4F42-8397-43BFE4669635', 'http://www.empireonline.com', 'Empire');
INSERT INTO sites (id, url, title) VALUES ('3DE7EA64-9DB0-4D68-989A-74302FD84A62', 'http://www.rottentomatoes.com', 'Rotten Tomatoes');
INSERT INTO sites (id, url, title) VALUES ('6BE67089-19FC-45DC-9588-6141C3119039', 'http://www.slashfilm.com', 'Slashfilm');
INSERT INTO sites (id, url, title) VALUES ('76ACBEF0-0070-4182-90A7-47B8BFC44A6F', 'http://www.totalfilm.com', 'Total Film');
INSERT INTO sites (id, url, title) VALUES ('F96A35FD-1BED-42ED-8A29-E907E4FE8CF4', 'http://www.yahoo.com', 'Yahoo');
INSERT INTO sites (id, url, title) VALUES ('80EBBC2F-8177-4643-88A8-EC4F5A5028FB', 'http://www.ap.org', 'Associated Press');
INSERT INTO sites (id, url, title) VALUES ('0E01F866-1039-46F2-8483-9D7B3D13E95A', 'http://www.CNNMoney.com', 'CNN Money');
INSERT INTO sites (id, url, title) VALUES ('1AA696F6-2805-4C4F-BB04-2FA82E7D27C6', 'http://www.ft.com', 'Financial Times');
INSERT INTO sites (id, url, title) VALUES ('0B2A190E-BE4F-401D-8FAF-7E09D49E3AFA', 'http://www.MarketWatch.com', 'MarketWatch.com');
INSERT INTO sites (id, url, title) VALUES ('1FB6C99D-1D16-400E-9C72-1F886CFED693', 'http://www.uk.reuters.com', 'Reuters');
INSERT INTO sites (id, url, title) VALUES ('78E794B0-F3AF-4A29-B892-36B48DCEBA6B', 'http://www.computerandvideogames.com', 'CVG');
INSERT INTO sites (id, url, title) VALUES ('A3DD9C7D-C626-4758-8F01-606500468A72', 'http://www.destructoid.com', 'Destructoid');
INSERT INTO sites (id, url, title) VALUES ('D627DB03-E603-47FD-AC1A-54C7466122D2', 'http://www.kotaku.com', 'Kotaku');
INSERT INTO sites (id, url, title) VALUES ('669C3B0A-44CC-4D92-BFC6-10CC85B64155', 'http://www.massively.com', 'Massively');
INSERT INTO sites (id, url, title) VALUES ('6445C27A-CC93-4770-8F66-92295EEDB721', 'http://www.shacknews.com', 'Shacknews');
INSERT INTO sites (id, url, title) VALUES ('DC01AB83-58AA-4115-B5E7-64A0971AD0B6', 'http://www.ew.com', 'Entertainment Weekly');
INSERT INTO sites (id, url, title) VALUES ('7BCBE1BB-9E72-4A4A-96D6-CBEEA5D2D42F', 'http://www.eonline.com', 'E! Online');
INSERT INTO sites (id, url, title) VALUES ('7435A9B5-BB2B-494C-8707-7DED42F73CC6', 'http://www.music.aol.com', 'AOL Music');
INSERT INTO sites (id, url, title) VALUES ('61C01117-CCDD-4519-AAA5-36218EAC6D82', 'http://www.mtv.com', 'MTV');
INSERT INTO sites (id, url, title) VALUES ('6B4BCA53-F934-422C-987C-0D336B86AB98', 'http://www.popeater.com', 'Popeater');
INSERT INTO sites (id, url, title) VALUES ('5B87C25E-6C61-40A7-BE3B-5C64534AF331', 'http://www.timesonline.co.uk', 'Timesonline');
INSERT INTO sites (id, url, title) VALUES ('97A6E038-5970-4E09-8E60-64C57F8D0D1D', 'http://www.msnbc.msn.com', 'MSNBC');
INSERT INTO sites (id, url, title) VALUES ('07F60827-BDBE-49C1-A9BC-53CCBB6037AC', 'http://www.news.google.co.uk', 'Google News');
INSERT INTO sites (id, url, title) VALUES ('B7B3BF80-1C1E-4642-B519-FCA0DBEA7E0C', 'http://www.bbcfocusmagazine.com', 'Focus Magazine');
INSERT INTO sites (id, url, title) VALUES ('66EAEBA5-B17B-4FF6-A328-ED74A7DB0DEB', 'http://www.newscientist.com', 'New Scientist');
INSERT INTO sites (id, url, title) VALUES ('A00CE5DA-AF68-4FDA-95A8-6AAA3C0B24C5', 'http://www.popsci.com', 'Popular Science');
INSERT INTO sites (id, url, title) VALUES ('2FE57DC6-407B-45DB-ACE7-3E71455807AD', 'http://www.sciencedaily.com', 'Science Daily');
INSERT INTO sites (id, url, title) VALUES ('64A11041-C45F-486D-AF32-6C66BA4BC25A', 'http://www.scientificamerican.com', 'Scientific America');
INSERT INTO sites (id, url, title) VALUES ('3F8FF9D1-ADAE-48D8-B02C-DA7032622AB2', 'http://www.digg.com', 'Digg');
INSERT INTO sites (id, url, title) VALUES ('6B518004-19A9-43B4-8DF0-495DDF78988C', 'http://www.uk.eurosport.yahoo.com', 'Eurosport');
INSERT INTO sites (id, url, title) VALUES ('955C1A29-FB79-44F6-8E89-FDBC4EEB64BE', 'http://www.skysports.com', 'Sky Sports');
INSERT INTO sites (id, url, title) VALUES ('8A4CE7F4-FDE7-497A-8D04-96B6062A1705', 'http://www.sportsillustrated.com', 'Sports Illustrated');
INSERT INTO sites (id, url, title) VALUES ('0679E8C0-FECD-4175-A96D-3D7C6BD06653', 'http://www.arstechnica.com', 'Ars Technica');
INSERT INTO sites (id, url, title) VALUES ('78FB160C-97FE-46C1-9532-12FBC29D65AA', 'http://www.engadget.com', 'Engadget');
INSERT INTO sites (id, url, title) VALUES ('2C3EDA7B-CC93-457C-BDD7-782AC615A9E0', 'http://www.slashdot.org', 'Slashdot');
INSERT INTO sites (id, url, title) VALUES ('3A3FCEA3-31CF-49A0-A685-52032319627F', 'http://www.techeblog.com', 'Techeblog');
INSERT INTO sites (id, url, title) VALUES ('8FB41C88-0ED3-4E9F-A0DC-C2595507247B', 'http://www.yankodesign.com', 'Yanko Design');
INSERT INTO sites (id, url, title) VALUES ('88A3E341-CD3D-4053-8352-8C4C58945840', 'http://www.concierge.com', 'Concierge.com');
INSERT INTO sites (id, url, title) VALUES ('0A300B6D-64EC-403A-BE1E-047E40FB6EFA', 'http://www.fodors.com', 'Fodor''s');
INSERT INTO sites (id, url, title) VALUES ('D78C71F0-CBE7-4846-9930-485E574B259A', 'http://www.lonelyplanet.com', 'Lonely Planet');
INSERT INTO sites (id, url, title) VALUES ('6D799E67-D7A0-41EA-BBD1-8FCFD88827DE', 'http://www.nationalgeographic.co.uk', 'National Geographic');
INSERT INTO sites (id, url, title) VALUES ('96F4F818-3F1E-42CE-B88D-5E4E62F1B490', 'http://www.travelandleisure.com', 'TravelandLeisure.com');
