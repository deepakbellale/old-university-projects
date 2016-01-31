﻿Create table m_sotrydnik
(
sotrydnik_number NUMERIC(20)  IDENTITY PRIMARY KEY,
sotrydnik_FIO VARCHAR (100),
sotrydnik_DataRozh DATETIME
); 

Create table m_transport
( 
sotrydnik_passport NUMERIC(20) REFERENCES m_sotrydnik (sotrydnik_number),
transport_number VARCHAR(50),
transport_marka VARCHAR (50)
CONSTRAINT pk_nomermarka PRIMARY KEY (transport_number, transport_marka)
);

Create table m_klient
(
klient_number  NUMERIC(30) IDENTITY PRIMARY KEY,
klient_adress VARCHAR (30),
klient_skidka NUMERIC (10)
);

Create table m_dostavka
( 
dostavka_number NUMERIC(30) IDENTITY PRIMARY KEY,
dostavka_datezakl DATETIME,
dostavka_datedost DATETIME,
dostavka_summa NUMERIC(10,3),
sotrydnik_number NUMERIC(20),
klient_number  NUMERIC(30)
CONSTRAINT fk_passport  FOREIGN KEY (sotrydnik_number) REFERENCES m_sotrydnik(sotrydnik_number),
CONSTRAINT fk_numkl FOREIGN KEY (klient_number) REFERENCES m_klient(klient_number)
);

Create table m_tip
(
tip_number NUMERIC(30) IDENTITY PRIMARY KEY, 
tip_nazvanie VARCHAR (30)
);

Create table m_gruz
(
gruz_number NUMERIC(30) IDENTITY PRIMARY KEY,
dostavka_number NUMERIC(30),
tip_number NUMERIC(30)
CONSTRAINT fk_dost  FOREIGN KEY (dostavka_number) REFERENCES m_dostavka(dostavka_number),
CONSTRAINT fk_tip FOREIGN KEY (tip_number) REFERENCES m_tip(tip_number)
);