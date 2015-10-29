IF EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('User_Lhm'))
BEGIN;
    DROP TABLE [User_Lhm];
END;

IF EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('Position_Lhm'))
BEGIN;
    DROP TABLE [Position_Lhm];
END;

IF EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('User'))
BEGIN;
    DROP TABLE [User];
END;

IF EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('Department'))
BEGIN;
    DROP TABLE [Department];
END;

IF EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('Position'))
BEGIN;
    DROP TABLE [Position];
END;

CREATE TABLE [Department] (
    [ID] INTEGER NOT NULL IDENTITY(1, 1),
    [Name] VARCHAR(50) NULL,    
    PRIMARY KEY ([ID])
);

CREATE TABLE [Position] (
    [ID] INTEGER NOT NULL,
    [Name] VARCHAR(50) NULL,   
    PRIMARY KEY ([ID])
);

CREATE TABLE [User] (
    [UserID] INTEGER NOT NULL IDENTITY(1, 1),
    [Username] VARCHAR(MAX) NULL,
    [Email] VARCHAR(255) NULL,
    [FirstName] VARCHAR(255) NULL,
    [lastName] VARCHAR(255) NULL,
    [Telephone] VARCHAR(100) NULL,
    [IsVIP] INTEGER NULL,
	[DepartmentID] INT NOT NULL,
	[PositionID] INT NOT NULL,
    PRIMARY KEY ([UserID])
);

ALTER TABLE [User]  WITH CHECK ADD  CONSTRAINT [FK_dbo.User_dbo.Department] FOREIGN KEY(DepartmentID)
REFERENCES [dbo].[Department] ([ID])

ALTER TABLE [User]  WITH CHECK ADD  CONSTRAINT [FK_dbo.User_dbo.Position] FOREIGN KEY(PositionID)
REFERENCES [dbo].[Position] ([ID])

INSERT INTO [Department] (NAME) VALUES ('Shipping'),('Sales'),('IT')

INSERT INTO [Position] (ID,NAME) VALUES (1,'Director'),(2,'Manager'),(3,'IT Support')

INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES ('bibendum','enim.Etiam@congueturpisIn.com','Connor','Carolyn','076 0197 2527',0,1,1),('lacus','lacinia.Sed@lacusQuisquepurus.net','Amber','Whilemina','055 7295 0418',0,2,2),('lorem','mattis.ornare.lectus@eleifendnuncrisus.net','Keefe','Nadine','0906 539 7133',0,3,3),('Donec','dictum@faucibus.org','Aurelia','Penelope','07731 810033',0,1,1),('ut','elementum@veliteusem.org','Lillith','Tallulah','055 1399 2623',0,1,1),('quis','Integer.urna.Vivamus@sit.com','Guinevere','Alexandra','0845 46 48',0,2,1),('imperdiet','et@purus.ca','Tatiana','Raphael','(0181) 414 2616',0,1,3),('diam','Donec.tincidunt.Donec@cursusluctusipsum.com','Barrett','Aline','(023) 3187 4269',0,2,2),('pellentesque.','libero.Donec@cubilia.org','Zeph','Velma','0912 191 4759',0,1,3),('dolor','non.magna@necmaurisblandit.ca','Willa','Tallulah','0800 983 2097',0,3,3);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('dui.','erat.Sed@sedhendrerit.edu','Latifah','Keane','0942 926 0441',0,3,2),('sed','est.Mauris.eu@ametluctus.com','Nyssa','Allegra','07475 488951',0,2,1),('dolor','amet@enimCurabitur.com','Britanney','Taylor','070 8850 6112',0,2,1),('lorem','dapibus.id@odio.com','Lydia','Chanda','0352 983 5650',0,1,2),('egestas','dictum@ametdapibus.org','Cullen','Isabella','0899 526 6133',0,3,3),('ac','Vivamus@Nullamsuscipitest.com','Amery','Abigail','(01929) 506754',0,1,2),('nulla','Proin.vel@felis.net','Maggy','Ina','07742 788286',0,2,3),('nulla.','amet.massa@elitNulla.com','Jaquelyn','Carson','(012211) 84665',0,1,3),('mi,','pede@penatibusetmagnis.edu','Kasper','Seth','07624 235264',0,3,2),('aliquet.','sapien@Praesenteu.com','Unity','Pandora','(01476) 11368',0,1,3);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('Phasellus','Maecenas.iaculis.aliquet@porttitorerosnec.ca','Pandora','Felicia','0893 293 1326',0,2,3),('massa','magna.Sed@orciUtsemper.com','Neve','Nehru','076 6117 2203',0,2,1),('semper','mollis.lectus.pede@pharetra.ca','Iola','Whitney','0800 577540',0,1,2),('eleifend','Aliquam@magnisdisparturient.org','Althea','Thor','0800 1111',0,1,3),('ultrices.','sagittis.felis@egetmagnaSuspendisse.com','Calvin','Stephanie','(017030) 82600',0,1,3),('non','mollis.nec.cursus@semperauctorMauris.net','Macon','Daria','055 1712 1055',0,2,1),('mauris','ligula.Aenean.gravida@atlacusQuisque.net','Autumn','Jerome','(015584) 06624',0,3,1),('Aenean','eget.tincidunt@sed.co.uk','Angelica','Noble','0800 884 0299',0,1,2),('elit,','ut@aclibero.net','Mason','Xanthus','(028) 8614 9668',0,1,2),('fermentum','Fusce.feugiat@Pellentesque.net','Brielle','Sean','(016977) 6915',0,2,3);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('metus.','sociis.natoque.penatibus@nequetellus.org','Noelle','Shelby','(020) 2520 0298',0,3,2),('nisl','feugiat.tellus.lorem@ipsumportaelit.org','Castor','Henry','(020) 5665 1039',0,3,1),('posuere','sed@tinciduntnuncac.net','Claudia','Barclay','0343 300 4264',0,1,3),('Duis','facilisis.Suspendisse.commodo@vitaealiquam.ca','Irma','Sade','(01011) 60116',0,2,1),('tempus','erat@ettristique.net','Blossom','Gavin','055 9979 2296',0,2,3),('ac','risus.odio.auctor@nec.net','Kameko','Yuli','0845 46 49',0,2,2),('Integer','ornare.lectus.ante@purussapien.ca','Abigail','Laura','(01265) 59777',0,3,1),('ornare,','euismod@mauris.org','Lisandra','Lois','0500 784622',0,3,2),('mauris,','semper.cursus.Integer@adipiscingMaurismolestie.ca','Berk','Brynne','0338 409 6239',0,3,3),('pharetra.','adipiscing@imperdieterat.org','Channing','Evelyn','076 0694 5082',0,3,2);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('cursus.','vestibulum.nec.euismod@tinciduntnibh.net','Lawrence','Christen','0500 050970',0,1,3),('commodo','et.rutrum.eu@sitametluctus.org','Malik','Declan','076 2569 4675',0,1,2),('turpis','accumsan.interdum.libero@nonleo.ca','Kalia','Cassidy','0845 46 48',0,2,3),('iaculis','sem.elit@ipsumDonecsollicitudin.edu','Walter','Drake','0307 537 4143',0,3,2),('Cum','Integer.mollis.Integer@Fuscefermentum.net','Orlando','Blake','056 5060 1207',0,1,3),('fringilla,','facilisi.Sed@faucibusleo.ca','Glenna','Venus','0500 295850',0,2,1),('arcu.','vitae.sodales@sociis.co.uk','Bo','Madaline','0822 231 9859',0,3,2),('vitae,','Vivamus.sit@nibhPhasellus.com','Victor','Macey','0800 010 9589',0,2,3),('facilisis.','aliquam.arcu@Maurisblanditenim.co.uk','Honorato','Solomon','055 1237 0430',0,2,1),('arcu.','iaculis@purus.co.uk','Lavinia','Wanda','07067 490287',0,3,2);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('lorem,','quis.accumsan.convallis@insodales.ca','Otto','Molly','0820 962 6277',0,1,1),('egestas','Praesent@dui.net','Bell','Zelenia','(021) 8567 3134',0,2,3),('eu,','amet.ultricies@ornareelit.com','Honorato','Brandon','055 0575 4465',0,3,2),('erat,','Etiam@mollisInteger.org','Porter','Cain','055 7039 2215',0,1,2),('sem','diam.Pellentesque.habitant@luctusutpellentesque.org','Emery','Quail','0500 610047',0,1,1),('ac','tempus.scelerisque.lorem@Aliquam.net','Ashely','Abdul','(01204) 729013',0,2,2),('ullamcorper,','risus@mollis.org','Charles','Jaime','076 6736 5756',0,3,1),('iaculis','orci.consectetuer.euismod@Aliquamornarelibero.co.uk','Xyla','Karina','0962 065 1551',0,1,2),('sem,','sagittis.Nullam.vitae@euduiCum.co.uk','Hannah','Zahir','(0161) 826 5211',0,2,3),('Mauris','ut@Aeneanegestashendrerit.org','Allistair','Paul','070 7274 9510',0,3,2);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('adipiscing','in.faucibus@nec.co.uk','Melanie','Calista','070 2061 6168',0,3,2),('lobortis,','Vestibulum.ante@acmi.org','Mari','Clark','055 1496 2222',0,1,3),('sollicitudin','sociis.natoque.penatibus@Integeraliquamadipiscing.co.uk','Ila','August','07624 993301',0,1,2),('natoque','lectus.pede@lectuspedeultrices.edu','Ishmael','Ivan','0500 265943',0,1,1),('Quisque','gravida.non@nisiCum.net','Karina','Rose','(01647) 730659',0,1,2),('nisl','tristique.ac@egestasSed.com','Roth','Morgan','07624 335206',0,3,3),('sed','Morbi@nulla.net','Tana','Barrett','07624 445282',0,3,3),('ridiculus','Nullam.feugiat.placerat@feliseget.co.uk','Merritt','Amy','07624 585694',0,3,1),('mus.','Nullam.feugiat@dolor.com','Helen','Juliet','070 3637 7270',0,1,2),('feugiat','mi@Fuscefermentumfermentum.co.uk','Quinn','Geoffrey','0951 346 6096',0,2,3);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('ornare,','Duis.elementum@Quisquetincidunt.org','Audrey','Colette','(01814) 290328',0,2,1),('Curabitur','Cras.dolor@sed.edu','Tashya','Karina','0939 127 4981',0,1,2),('lorem,','Vestibulum.accumsan@lacus.edu','Rama','Harper','056 4209 5817',0,3,3),('conubia','purus.ac.tellus@tortornibhsit.edu','Cameran','Keelie','07624 880452',0,3,2),('Nulla','suscipit@Cum.org','Helen','Matthew','07624 123217',0,1,1),('lorem,','dui@luctus.net','Howard','Idola','0845 46 41',0,2,3),('sociis','magna.Cras.convallis@Namac.org','Yoshio','Wade','(017646) 71543',0,2,1),('turpis','ridiculus.mus@dolor.ca','Lewis','Megan','(014489) 25483',0,1,2),('in,','inceptos@enimEtiam.co.uk','Stone','Genevieve','056 6565 1616',0,1,2),('Duis','lorem@magna.net','Willow','Megan','0500 453191',0,3,3);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('magnis','libero.at@nonlobortisquis.ca','Orson','Cassidy','0500 835574',0,1,1),('In','sapien@mipedenonummy.ca','Abigail','Serina','07218 292547',0,3,2),('pulvinar','Lorem@utodiovel.co.uk','Zahir','Harriet','0800 702792',0,2,3),('ac','enim.diam@feugiatLorem.ca','Laura','Aquila','(028) 4485 5761',0,1,2),('nec,','sociosqu@eumetusIn.ca','Blaze','Harding','0800 384166',0,1,1),('mattis','malesuada.vel.venenatis@ipsumdolorsit.co.uk','Russell','Joel','(0117) 044 0767',0,2,2),('mauris.','tellus@Donecfeugiat.net','Maris','Alma','0374 712 8667',0,3,3),('varius.','ac.tellus.Suspendisse@rutrumeuultrices.edu','Kay','Gemma','070 5742 4914',0,2,2),('vulputate','auctor.Mauris@Etiamlaoreet.ca','Jorden','Otto','056 0956 1186',0,3,1),('lacus.','Quisque.libero@laoreet.co.uk','Chase','Ray','(016625) 74283',0,2,2);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP],[DepartmentID],[PositionID]) VALUES('Duis','tincidunt.neque@Utsemperpretium.org','Evelyn','Davis','0346 572 3564',0,2,1),('rutrum,','mattis.velit@dictumeu.net','Jaquelyn','Kennan','055 3809 7351',0,2,3),('auctor','Sed.pharetra.felis@consectetuermauris.edu','Minerva','Fulton','0845 46 40',0,1,2),('consectetuer','eu@orciluctus.com','Ann','Jane','(01167) 425495',0,1,1),('ante.','vitae.sodales.at@diamat.co.uk','Chiquita','Abbot','(01874) 124025',0,1,2),('Nulla','lorem.fringilla.ornare@molestieSed.com','Wilma','Hakeem','(01140) 126484',0,2,1),('eros.','tellus@turpisIn.ca','Bertha','Whitney','0985 033 1981',0,3,3),('lorem.','senectus.et@nondui.ca','Vaughan','Yasir','0845 46 44',0,1,2),('risus.','consectetuer@laoreetipsum.ca','Alea','Joel','056 4851 8410',0,2,2),('tempus','cursus.purus@venenatislacus.com','Nadine','Logan','070 8022 5867',0,2,1);
