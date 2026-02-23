-- ============================================================
-- PCConfigurator — полная очистка и заполнение тестовыми данными
-- MS SQL Server
-- ============================================================

USE PCConfigurator;
GO

-- ============================================================
-- 1. ОЧИСТКА (порядок важен — от зависимых к независимым)
-- ============================================================

DELETE FROM ConfigurationStorage;
DELETE FROM ConfigurationRAM;
DELETE FROM Configurations;
DELETE FROM Processors;
DELETE FROM Motherboards;
DELETE FROM RAM;
DELETE FROM GPUs;
DELETE FROM Storage;
DELETE FROM Manufacturers;

-- Сброс счётчиков IDENTITY
DBCC CHECKIDENT ('ConfigurationStorage', RESEED, 0);
DBCC CHECKIDENT ('ConfigurationRAM',     RESEED, 0);
DBCC CHECKIDENT ('Configurations',       RESEED, 0);
DBCC CHECKIDENT ('Processors',           RESEED, 0);
DBCC CHECKIDENT ('Motherboards',         RESEED, 0);
DBCC CHECKIDENT ('RAM',                  RESEED, 0);
DBCC CHECKIDENT ('GPUs',                 RESEED, 0);
DBCC CHECKIDENT ('Storage',              RESEED, 0);
DBCC CHECKIDENT ('Manufacturers',        RESEED, 0);

-- ============================================================
-- 2. ПРОИЗВОДИТЕЛИ (12 строк)
--    ID 1–4  : процессоры/чипсеты
--    ID 5–7  : платы
--    ID 8–9  : ОЗУ
--    ID 10   : GPU (NVIDIA)
--    ID 11   : GPU (AMD)
--    ID 12   : накопители
-- ============================================================

INSERT INTO Manufacturers (Name, Country, Website) VALUES
('Intel',          'США',   'https://www.intel.com'),       -- 1
('AMD',            'США',   'https://www.amd.com'),         -- 2
('NVIDIA',         'США',   'https://www.nvidia.com'),      -- 3
('ASUS',           'Тайвань','https://www.asus.com'),        -- 4
('MSI',            'Тайвань','https://www.msi.com'),         -- 5
('Gigabyte',       'Тайвань','https://www.gigabyte.com'),    -- 6
('Corsair',        'США',   'https://www.corsair.com'),     -- 7
('Kingston',       'США',   'https://www.kingston.com'),    -- 8
('Samsung',        'Корея', 'https://www.samsung.com'),     -- 9
('Seagate',        'США',   'https://www.seagate.com'),     -- 10
('Western Digital','США',   'https://www.westerndigital.com'), -- 11
('Crucial',        'США',   'https://www.crucial.com');     -- 12

-- ============================================================
-- 3. ПРОЦЕССОРЫ (20 строк)
--    LGA1700 (Intel, ManufacturerId=1): ID 1–10
--    AM4     (AMD,   ManufacturerId=2): ID 11–15
--    AM5     (AMD,   ManufacturerId=2): ID 16–20
-- ============================================================

INSERT INTO Processors (ManufacturerId, Model, Socket, Cores, Frequency) VALUES
-- Intel LGA1700 (совместимы с платами 1–10)
(1, 'Core i3-12100',  'LGA1700',  4,  3.30),
(1, 'Core i5-12400',  'LGA1700',  6,  2.50),
(1, 'Core i5-12600K', 'LGA1700', 10,  3.70),
(1, 'Core i7-12700K', 'LGA1700', 12,  3.60),
(1, 'Core i9-12900K', 'LGA1700', 16,  3.20),
(1, 'Core i3-13100',  'LGA1700',  4,  3.40),
(1, 'Core i5-13400',  'LGA1700', 10,  2.50),
(1, 'Core i5-13600K', 'LGA1700', 14,  3.50),
(1, 'Core i7-13700K', 'LGA1700', 16,  3.40),
(1, 'Core i9-13900K', 'LGA1700', 24,  3.00),
-- AMD AM4 (совместимы с платами 11–15)
(2, 'Ryzen 3 3300X',  'AM4',  4,  3.80),
(2, 'Ryzen 5 3600',   'AM4',  6,  3.60),
(2, 'Ryzen 5 5600X',  'AM4',  6,  3.70),
(2, 'Ryzen 7 5700X',  'AM4',  8,  3.40),
(2, 'Ryzen 9 5900X',  'AM4', 12,  3.70),
-- AMD AM5 (совместимы с платами 16–20)
(2, 'Ryzen 5 7600',   'AM5',  6,  3.80),
(2, 'Ryzen 5 7600X',  'AM5',  6,  4.70),
(2, 'Ryzen 7 7700X',  'AM5',  8,  4.50),
(2, 'Ryzen 9 7900X',  'AM5', 12,  4.70),
(2, 'Ryzen 9 7950X',  'AM5', 16,  4.50);

-- ============================================================
-- 4. МАТЕРИНСКИЕ ПЛАТЫ (20 строк)
--    ID 1–5  : LGA1700 + DDR4  → процессоры 1–10, ОЗУ DDR4
--    ID 6–10 : LGA1700 + DDR5  → процессоры 1–10, ОЗУ DDR5
--    ID 11–15: AM4     + DDR4  → процессоры 11–15, ОЗУ DDR4
--    ID 16–20: AM5     + DDR5  → процессоры 16–20, ОЗУ DDR5
-- ============================================================

INSERT INTO Motherboards (ManufacturerId, Model, Socket, Chipset, RAMType, MaxRAM) VALUES
-- LGA1700 + DDR4
(4, 'PRIME H670-PLUS D4',      'LGA1700', 'H670',  'DDR4', 128),
(5, 'PRO H610M-G DDR4',        'LGA1700', 'H610',  'DDR4',  64),
(6, 'Z690 UD DDR4',            'LGA1700', 'Z690',  'DDR4', 128),
(4, 'TUF GAMING Z690-PLUS D4', 'LGA1700', 'Z690',  'DDR4', 128),
(5, 'MAG Z690 TOMAHAWK DDR4',  'LGA1700', 'Z690',  'DDR4', 128),
-- LGA1700 + DDR5
(4, 'ROG MAXIMUS Z790 HERO',   'LGA1700', 'Z790',  'DDR5', 192),
(5, 'MEG Z790 ACE',            'LGA1700', 'Z790',  'DDR5', 192),
(6, 'Z790 AORUS MASTER',       'LGA1700', 'Z790',  'DDR5', 192),
(4, 'PRIME Z790-P WIFI',       'LGA1700', 'Z790',  'DDR5', 192),
(5, 'PRO Z790-A WIFI',         'LGA1700', 'Z790',  'DDR5', 192),
-- AM4 + DDR4
(4, 'ROG CROSSHAIR VIII HERO', 'AM4',     'X570',  'DDR4', 128),
(5, 'MAG X570S TORPEDO MAX',   'AM4',     'X570',  'DDR4', 128),
(6, 'B550 AORUS PRO AX',       'AM4',     'B550',  'DDR4', 128),
(4, 'TUF GAMING B550-PLUS',    'AM4',     'B550',  'DDR4', 128),
(5, 'B450 TOMAHAWK MAX II',    'AM4',     'B450',  'DDR4', 128),
-- AM5 + DDR5
(4, 'ROG CROSSHAIR X670E HERO','AM5',     'X670E', 'DDR5', 192),
(5, 'MEG X670E ACE',           'AM5',     'X670E', 'DDR5', 192),
(6, 'X670E AORUS MASTER',      'AM5',     'X670E', 'DDR5', 192),
(4, 'PRIME X670-P WIFI',       'AM5',     'X670',  'DDR5', 192),
(5, 'PRO X670-P WIFI',         'AM5',     'X670',  'DDR5', 192);

-- ============================================================
-- 5. ОПЕРАТИВНАЯ ПАМЯТЬ (20 строк)
--    DDR4 (ID 1–10)  → платы 1–5 и 11–15
--    DDR5 (ID 11–20) → платы 6–10 и 16–20
-- ============================================================

INSERT INTO RAM (ManufacturerId, Model, Type, Capacity, Frequency) VALUES
-- DDR4
(7,  'Vengeance LPX 8GB 3200',     'DDR4',  8, 3200),
(7,  'Vengeance LPX 16GB 3200',    'DDR4', 16, 3200),
(7,  'Dominator Platinum 32GB 3600','DDR4', 32, 3600),
(8,  'HyperX Fury 8GB 2666',       'DDR4',  8, 2666),
(8,  'HyperX Fury 16GB 3200',      'DDR4', 16, 3200),
(9,  'Samsung 8GB 2666',           'DDR4',  8, 2666),
(9,  'Samsung 16GB 3200',          'DDR4', 16, 3200),
(12, 'Ballistix 8GB 3600',         'DDR4',  8, 3600),
(12, 'Ballistix 16GB 3600',        'DDR4', 16, 3600),
(8,  'ValueRAM 16GB 2400',         'DDR4', 16, 2400),
-- DDR5
(7,  'Vengeance DDR5 16GB 5200',   'DDR5', 16, 5200),
(7,  'Vengeance DDR5 32GB 5200',   'DDR5', 32, 5200),
(7,  'Dominator Titanium 32GB 6000','DDR5',32, 6000),
(8,  'Fury Beast DDR5 16GB 5600',  'DDR5', 16, 5600),
(8,  'Fury Beast DDR5 32GB 5600',  'DDR5', 32, 5600),
(8,  'Fury Renegade DDR5 32GB 6400','DDR5',32, 6400),
(9,  'Samsung DDR5 16GB 4800',     'DDR5', 16, 4800),
(9,  'Samsung DDR5 32GB 5200',     'DDR5', 32, 5200),
(12, 'Crucial DDR5 16GB 4800',     'DDR5', 16, 4800),
(12, 'Crucial DDR5 32GB 4800',     'DDR5', 32, 4800);

-- ============================================================
-- 6. ВИДЕОКАРТЫ (20 строк)
--    NVIDIA (ManufacturerId=3): ID 1–12 — на платах ASUS/MSI/Gigabyte
--    AMD    (ManufacturerId=2): ID 13–20
-- ============================================================

INSERT INTO GPUs (ManufacturerId, Model, Memory, MemoryType, PowerConsumption) VALUES
-- NVIDIA RTX 30xx
(4, 'RTX 3060 OC',         12, 'GDDR6',  170),
(5, 'RTX 3060 Ti GAMING',   8, 'GDDR6X', 200),
(6, 'RTX 3070 EAGLE OC',    8, 'GDDR6',  220),
(4, 'RTX 3070 Ti TUF OC',   8, 'GDDR6X', 290),
(5, 'RTX 3080 GAMING X',   10, 'GDDR6X', 320),
(4, 'RTX 3090 STRIX OC',   24, 'GDDR6X', 350),
-- NVIDIA RTX 40xx
(5, 'RTX 4060 VENTUS 2X',   8, 'GDDR6',  115),
(4, 'RTX 4060 Ti TUF OC',   8, 'GDDR6',  160),
(6, 'RTX 4070 EAGLE OC',   12, 'GDDR6X', 200),
(4, 'RTX 4070 Ti STRIX OC',12, 'GDDR6X', 285),
(5, 'RTX 4080 SUPRIM X',   16, 'GDDR6X', 320),
(4, 'RTX 4090 ROG STRIX',  24, 'GDDR6X', 450),
-- AMD RX 6000
(4, 'RX 6600 DUAL OC',      8, 'GDDR6',  132),
(5, 'RX 6650 XT MECH 2X',   8, 'GDDR6',  180),
(6, 'RX 6700 XT GAMING OC',12, 'GDDR6',  230),
(4, 'RX 6800 XT STRIX OC', 16, 'GDDR6',  300),
-- AMD RX 7000
(5, 'RX 7600 MECH 2X OC',   8, 'GDDR6',  165),
(4, 'RX 7700 XT TUF OC',   12, 'GDDR6',  245),
(6, 'RX 7800 XT GAMING OC',16, 'GDDR6',  263),
(6, 'RX 7900 XTX AORUS',   24, 'GDDR6',  355);

-- ============================================================
-- 7. НАКОПИТЕЛИ (20 строк)
--    NVMe SSD (ID 1–8)
--    SATA SSD (ID 9–14)
--    HDD      (ID 15–20)
-- ============================================================

INSERT INTO Storage (ManufacturerId, Model, Type, Capacity, Interface) VALUES
-- NVMe SSD
(9,  '970 EVO Plus 500GB',      'SSD',  500,  'NVMe'),
(9,  '970 EVO Plus 1TB',        'SSD', 1000,  'NVMe'),
(9,  '980 PRO 1TB',             'SSD', 1000,  'NVMe'),
(9,  '980 PRO 2TB',             'SSD', 2000,  'NVMe'),
(11, 'WD Black SN770 1TB',      'SSD', 1000,  'NVMe'),
(11, 'WD Black SN850X 1TB',     'SSD', 1000,  'NVMe'),
(12, 'P3 Plus 1TB',             'SSD', 1000,  'NVMe'),
(8,  'Fury Renegade 2TB',       'SSD', 2000,  'NVMe'),
-- SATA SSD
(9,  '870 EVO 500GB',           'SSD',  500,  'SATA'),
(9,  '870 EVO 1TB',             'SSD', 1000,  'SATA'),
(12, 'MX500 500GB',             'SSD',  500,  'SATA'),
(12, 'MX500 1TB',               'SSD', 1000,  'SATA'),
(8,  'KC600 480GB',             'SSD',  480,  'SATA'),
(11, 'WD Green 480GB',          'SSD',  480,  'SATA'),
-- HDD
(10, 'Barracuda 1TB 7200rpm',   'HDD', 1000,  'SATA'),
(10, 'Barracuda 2TB 7200rpm',   'HDD', 2000,  'SATA'),
(10, 'Barracuda 4TB 7200rpm',   'HDD', 4000,  'SATA'),
(11, 'WD Blue 1TB 7200rpm',     'HDD', 1000,  'SATA'),
(11, 'WD Blue 2TB 5400rpm',     'HDD', 2000,  'SATA'),
(11, 'WD Red Plus 4TB',         'HDD', 4000,  'SATA');

-- ============================================================
-- ГОТОВО
-- Совместимые связки для сборки конфигураций:
--
--   Intel + DDR4:
--     Процессоры 1–10  + Платы 1–5   + ОЗУ 1–10  + GPU любой
--
--   Intel + DDR5:
--     Процессоры 1–10  + Платы 6–10  + ОЗУ 11–20 + GPU любой
--
--   AMD AM4 + DDR4:
--     Процессоры 11–15 + Платы 11–15 + ОЗУ 1–10  + GPU любой
--
--   AMD AM5 + DDR5:
--     Процессоры 16–20 + Платы 16–20 + ОЗУ 11–20 + GPU любой
--
--   Накопители: любой из Storage 1–20 (NVMe/SATA/HDD)
--   GPU: любой из GPUs 1–20 (необязателен)
-- ============================================================
