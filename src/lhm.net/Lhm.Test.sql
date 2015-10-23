IF EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('User'))
BEGIN;
    DROP TABLE [User];
END;
GO

CREATE TABLE [User] (
    [UserID] INTEGER NOT NULL IDENTITY(1, 1),
    [Username] VARCHAR(MAX) NULL,
    [Email] VARCHAR(255) NULL,
    [FirstName] VARCHAR(255) NULL,
    [lastName] VARCHAR(255) NULL,
    [Telephone] VARCHAR(100) NULL,
    [IsVIP] INTEGER NULL,
    PRIMARY KEY ([UserID])
);
GO

INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('bibendum','enim.Etiam@congueturpisIn.com','Connor','Carolyn','076 0197 2527',0),('lacus','lacinia.Sed@lacusQuisquepurus.net','Amber','Whilemina','055 7295 0418',0),('lorem','mattis.ornare.lectus@eleifendnuncrisus.net','Keefe','Nadine','0906 539 7133',0),('Donec','dictum@faucibus.org','Aurelia','Penelope','07731 810033',0),('ut','elementum@veliteusem.org','Lillith','Tallulah','055 1399 2623',0),('quis','Integer.urna.Vivamus@sit.com','Guinevere','Alexandra','0845 46 48',0),('imperdiet','et@purus.ca','Tatiana','Raphael','(0181) 414 2616',0),('diam','Donec.tincidunt.Donec@cursusluctusipsum.com','Barrett','Aline','(023) 3187 4269',0),('pellentesque.','libero.Donec@cubilia.org','Zeph','Velma','0912 191 4759',0),('dolor','non.magna@necmaurisblandit.ca','Willa','Tallulah','0800 983 2097',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('dui.','erat.Sed@sedhendrerit.edu','Latifah','Keane','0942 926 0441',0),('sed','est.Mauris.eu@ametluctus.com','Nyssa','Allegra','07475 488951',0),('dolor','amet@enimCurabitur.com','Britanney','Taylor','070 8850 6112',0),('lorem','dapibus.id@odio.com','Lydia','Chanda','0352 983 5650',0),('egestas','dictum@ametdapibus.org','Cullen','Isabella','0899 526 6133',0),('ac','Vivamus@Nullamsuscipitest.com','Amery','Abigail','(01929) 506754',0),('nulla','Proin.vel@felis.net','Maggy','Ina','07742 788286',0),('nulla.','amet.massa@elitNulla.com','Jaquelyn','Carson','(012211) 84665',0),('mi,','pede@penatibusetmagnis.edu','Kasper','Seth','07624 235264',0),('aliquet.','sapien@Praesenteu.com','Unity','Pandora','(01476) 11368',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('Phasellus','Maecenas.iaculis.aliquet@porttitorerosnec.ca','Pandora','Felicia','0893 293 1326',0),('massa','magna.Sed@orciUtsemper.com','Neve','Nehru','076 6117 2203',0),('semper','mollis.lectus.pede@pharetra.ca','Iola','Whitney','0800 577540',0),('eleifend','Aliquam@magnisdisparturient.org','Althea','Thor','0800 1111',0),('ultrices.','sagittis.felis@egetmagnaSuspendisse.com','Calvin','Stephanie','(017030) 82600',0),('non','mollis.nec.cursus@semperauctorMauris.net','Macon','Daria','055 1712 1055',0),('mauris','ligula.Aenean.gravida@atlacusQuisque.net','Autumn','Jerome','(015584) 06624',0),('Aenean','eget.tincidunt@sed.co.uk','Angelica','Noble','0800 884 0299',0),('elit,','ut@aclibero.net','Mason','Xanthus','(028) 8614 9668',0),('fermentum','Fusce.feugiat@Pellentesque.net','Brielle','Sean','(016977) 6915',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('metus.','sociis.natoque.penatibus@nequetellus.org','Noelle','Shelby','(020) 2520 0298',0),('nisl','feugiat.tellus.lorem@ipsumportaelit.org','Castor','Henry','(020) 5665 1039',0),('posuere','sed@tinciduntnuncac.net','Claudia','Barclay','0343 300 4264',0),('Duis','facilisis.Suspendisse.commodo@vitaealiquam.ca','Irma','Sade','(01011) 60116',0),('tempus','erat@ettristique.net','Blossom','Gavin','055 9979 2296',0),('ac','risus.odio.auctor@nec.net','Kameko','Yuli','0845 46 49',0),('Integer','ornare.lectus.ante@purussapien.ca','Abigail','Laura','(01265) 59777',0),('ornare,','euismod@mauris.org','Lisandra','Lois','0500 784622',0),('mauris,','semper.cursus.Integer@adipiscingMaurismolestie.ca','Berk','Brynne','0338 409 6239',0),('pharetra.','adipiscing@imperdieterat.org','Channing','Evelyn','076 0694 5082',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('cursus.','vestibulum.nec.euismod@tinciduntnibh.net','Lawrence','Christen','0500 050970',0),('commodo','et.rutrum.eu@sitametluctus.org','Malik','Declan','076 2569 4675',0),('turpis','accumsan.interdum.libero@nonleo.ca','Kalia','Cassidy','0845 46 48',0),('iaculis','sem.elit@ipsumDonecsollicitudin.edu','Walter','Drake','0307 537 4143',0),('Cum','Integer.mollis.Integer@Fuscefermentum.net','Orlando','Blake','056 5060 1207',0),('fringilla,','facilisi.Sed@faucibusleo.ca','Glenna','Venus','0500 295850',0),('arcu.','vitae.sodales@sociis.co.uk','Bo','Madaline','0822 231 9859',0),('vitae,','Vivamus.sit@nibhPhasellus.com','Victor','Macey','0800 010 9589',0),('facilisis.','aliquam.arcu@Maurisblanditenim.co.uk','Honorato','Solomon','055 1237 0430',0),('arcu.','iaculis@purus.co.uk','Lavinia','Wanda','07067 490287',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('lorem,','quis.accumsan.convallis@insodales.ca','Otto','Molly','0820 962 6277',0),('egestas','Praesent@dui.net','Bell','Zelenia','(021) 8567 3134',0),('eu,','amet.ultricies@ornareelit.com','Honorato','Brandon','055 0575 4465',0),('erat,','Etiam@mollisInteger.org','Porter','Cain','055 7039 2215',0),('sem','diam.Pellentesque.habitant@luctusutpellentesque.org','Emery','Quail','0500 610047',0),('ac','tempus.scelerisque.lorem@Aliquam.net','Ashely','Abdul','(01204) 729013',0),('ullamcorper,','risus@mollis.org','Charles','Jaime','076 6736 5756',0),('iaculis','orci.consectetuer.euismod@Aliquamornarelibero.co.uk','Xyla','Karina','0962 065 1551',0),('sem,','sagittis.Nullam.vitae@euduiCum.co.uk','Hannah','Zahir','(0161) 826 5211',0),('Mauris','ut@Aeneanegestashendrerit.org','Allistair','Paul','070 7274 9510',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('adipiscing','in.faucibus@nec.co.uk','Melanie','Calista','070 2061 6168',0),('lobortis,','Vestibulum.ante@acmi.org','Mari','Clark','055 1496 2222',0),('sollicitudin','sociis.natoque.penatibus@Integeraliquamadipiscing.co.uk','Ila','August','07624 993301',0),('natoque','lectus.pede@lectuspedeultrices.edu','Ishmael','Ivan','0500 265943',0),('Quisque','gravida.non@nisiCum.net','Karina','Rose','(01647) 730659',0),('nisl','tristique.ac@egestasSed.com','Roth','Morgan','07624 335206',0),('sed','Morbi@nulla.net','Tana','Barrett','07624 445282',0),('ridiculus','Nullam.feugiat.placerat@feliseget.co.uk','Merritt','Amy','07624 585694',0),('mus.','Nullam.feugiat@dolor.com','Helen','Juliet','070 3637 7270',0),('feugiat','mi@Fuscefermentumfermentum.co.uk','Quinn','Geoffrey','0951 346 6096',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('ornare,','Duis.elementum@Quisquetincidunt.org','Audrey','Colette','(01814) 290328',0),('Curabitur','Cras.dolor@sed.edu','Tashya','Karina','0939 127 4981',0),('lorem,','Vestibulum.accumsan@lacus.edu','Rama','Harper','056 4209 5817',0),('conubia','purus.ac.tellus@tortornibhsit.edu','Cameran','Keelie','07624 880452',0),('Nulla','suscipit@Cum.org','Helen','Matthew','07624 123217',0),('lorem,','dui@luctus.net','Howard','Idola','0845 46 41',0),('sociis','magna.Cras.convallis@Namac.org','Yoshio','Wade','(017646) 71543',0),('turpis','ridiculus.mus@dolor.ca','Lewis','Megan','(014489) 25483',0),('in,','inceptos@enimEtiam.co.uk','Stone','Genevieve','056 6565 1616',0),('Duis','lorem@magna.net','Willow','Megan','0500 453191',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('magnis','libero.at@nonlobortisquis.ca','Orson','Cassidy','0500 835574',0),('In','sapien@mipedenonummy.ca','Abigail','Serina','07218 292547',0),('pulvinar','Lorem@utodiovel.co.uk','Zahir','Harriet','0800 702792',0),('ac','enim.diam@feugiatLorem.ca','Laura','Aquila','(028) 4485 5761',0),('nec,','sociosqu@eumetusIn.ca','Blaze','Harding','0800 384166',0),('mattis','malesuada.vel.venenatis@ipsumdolorsit.co.uk','Russell','Joel','(0117) 044 0767',0),('mauris.','tellus@Donecfeugiat.net','Maris','Alma','0374 712 8667',0),('varius.','ac.tellus.Suspendisse@rutrumeuultrices.edu','Kay','Gemma','070 5742 4914',0),('vulputate','auctor.Mauris@Etiamlaoreet.ca','Jorden','Otto','056 0956 1186',0),('lacus.','Quisque.libero@laoreet.co.uk','Chase','Ray','(016625) 74283',0);
INSERT INTO [User]([Username],[Email],[FirstName],[lastName],[Telephone],[IsVIP]) VALUES('Duis','tincidunt.neque@Utsemperpretium.org','Evelyn','Davis','0346 572 3564',0),('rutrum,','mattis.velit@dictumeu.net','Jaquelyn','Kennan','055 3809 7351',0),('auctor','Sed.pharetra.felis@consectetuermauris.edu','Minerva','Fulton','0845 46 40',0),('consectetuer','eu@orciluctus.com','Ann','Jane','(01167) 425495',0),('ante.','vitae.sodales.at@diamat.co.uk','Chiquita','Abbot','(01874) 124025',0),('Nulla','lorem.fringilla.ornare@molestieSed.com','Wilma','Hakeem','(01140) 126484',0),('eros.','tellus@turpisIn.ca','Bertha','Whitney','0985 033 1981',0),('lorem.','senectus.et@nondui.ca','Vaughan','Yasir','0845 46 44',0),('risus.','consectetuer@laoreetipsum.ca','Alea','Joel','056 4851 8410',0),('tempus','cursus.purus@venenatislacus.com','Nadine','Logan','070 8022 5867',0);