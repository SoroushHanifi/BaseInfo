-- ===============================================
-- Sample Data for BaseInfo Project Demo
-- ===============================================

USE BaseInfoDb;
GO

-- حذف داده‌های قبلی (اختیاری)
DELETE FROM MainTitleServiceFeatures;
DELETE FROM ServiceFeatures;
DELETE FROM MainTitles;
DELETE FROM Scopes;
DELETE FROM Departments;

-- Reset Identity Seeds
DBCC CHECKIDENT ('MainTitleServiceFeatures', RESEED, 0);
DBCC CHECKIDENT ('ServiceFeatures', RESEED, 0);
DBCC CHECKIDENT ('MainTitles', RESEED, 0);
DBCC CHECKIDENT ('Scopes', RESEED, 0);
DBCC CHECKIDENT ('Departments', RESEED, 0);

-- ===============================================
-- 1. Departments (ادارات کل)
-- ===============================================

INSERT INTO Departments (Name, CreateUserId, CreateDate, ModifyDate, IsDelete)
VALUES 
(N'اداره کل درمان', '1234567890', GETDATE(), GETDATE(), 0),
(N'اداره کل پیشگیری', '1234567890', GETDATE(), GETDATE(), 0),
(N'اداره کل دارو و تجهیزات پزشکی', '1234567890', GETDATE(), GETDATE(), 0),
(N'اداره کل نظارت و بازرسی', '1234567890', GETDATE(), GETDATE(), 0),
(N'اداره کل آموزش پزشکی', '1234567890', GETDATE(), GETDATE(), 0),
(N'اداره کل تحقیقات و فناوری', '1234567890', GETDATE(), GETDATE(), 0);

-- ===============================================
-- 2. Scopes (حوزه‌ها)
-- ===============================================

INSERT INTO Scopes (Name, DepartmentId, CreateUserId, CreateDate, ModifyDate, IsDelete)
VALUES 
-- حوزه‌های اداره کل درمان
(N'بیمارستان‌های عمومی', 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'بیمارستان‌های تخصصی', 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'مراکز درمانی سرپایی', 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات اورژانس', 1, '1234567890', GETDATE(), GETDATE(), 0),

-- حوزه‌های اداره کل پیشگیری
(N'واکسیناسیون', 2, '1234567890', GETDATE(), GETDATE(), 0),
(N'مراقبت‌های بهداشتی', 2, '1234567890', GETDATE(), GETDATE(), 0),
(N'کنترل بیماری‌ها', 2, '1234567890', GETDATE(), GETDATE(), 0),

-- حوزه‌های اداره کل دارو و تجهیزات
(N'داروخانه‌ها', 3, '1234567890', GETDATE(), GETDATE(), 0),
(N'تجهیزات پزشکی', 3, '1234567890', GETDATE(), GETDATE(), 0),
(N'کیت‌های تشخیصی', 3, '1234567890', GETDATE(), GETDATE(), 0),

-- حوزه‌های اداره کل نظارت و بازرسی
(N'بازرسی مراکز درمانی', 4, '1234567890', GETDATE(), GETDATE(), 0),
(N'نظارت بر کیفیت خدمات', 4, '1234567890', GETDATE(), GETDATE(), 0),

-- حوزه‌های اداره کل آموزش پزشکی
(N'آموزش پزشکان', 5, '1234567890', GETDATE(), GETDATE(), 0),
(N'آموزش پرستاران', 5, '1234567890', GETDATE(), GETDATE(), 0),

-- حوزه‌های اداره کل تحقیقات و فناوری
(N'تحقیقات بالینی', 6, '1234567890', GETDATE(), GETDATE(), 0),
(N'فناوری‌های نوین درمانی', 6, '1234567890', GETDATE(), GETDATE(), 0);

-- ===============================================
-- 3. MainTitles (عناوین اصلی)
-- ===============================================

INSERT INTO MainTitles (Name, Description, Amount, ScopeId, DisplayOrder, CreateUserId, CreateDate, ModifyDate, IsDelete)
VALUES 
-- عناوین مربوط به بیمارستان‌های عمومی
(N'ارائه خدمات بستری', N'خدمات بستری بیماران در بخش‌های مختلف', 50000000.00, 1, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات جراحی عمومی', N'انجام عمل‌های جراحی عمومی', 75000000.00, 1, 2, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات زایمان طبیعی', N'مراقبت‌های مامایی و زایمان طبیعی', 25000000.00, 1, 3, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به بیمارستان‌های تخصصی
(N'جراحی قلب و عروق', N'عمل‌های جراحی تخصصی قلب و عروق', 150000000.00, 2, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'پیوند اعضا', N'خدمات پیوند کلیه، کبد و سایر اعضا', 200000000.00, 2, 2, '1234567890', GETDATE(), GETDATE(), 0),
(N'درمان سرطان', N'شیمی‌درمانی و رادیوتراپی', 120000000.00, 2, 3, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به مراکز درمانی سرپایی
(N'ویزیت تخصصی', N'ویزیت پزشکان متخصص', 5000000.00, 3, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات آزمایشگاهی', N'انجام آزمایش‌های تشخیص طبی', 3000000.00, 3, 2, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات رادیولوژی', N'عکسبرداری و سونوگرافی', 8000000.00, 3, 3, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به خدمات اورژانس
(N'مراقبت‌های ویژه', N'بخش مراقبت‌های ویژه اورژانس', 35000000.00, 4, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات آمبولانس', N'حمل و نقل بیماران اورژانسی', 15000000.00, 4, 2, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به واکسیناسیون
(N'واکسیناسیون کودکان', N'برنامه واکسیناسیون ملی کودکان', 20000000.00, 5, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'واکسیناسیون بزرگسالان', N'واکسن‌های فصلی و ضروری بزرگسالان', 15000000.00, 5, 2, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به مراقبت‌های بهداشتی
(N'مراقبت مادر و کودک', N'خدمات بهداشتی مادران باردار و کودکان', 18000000.00, 6, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'کنترل بیماری‌های مزمن', N'مراقبت از بیماران دیابتی و فشار خون', 22000000.00, 6, 2, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به کنترل بیماری‌ها
(N'کنترل بیماری‌های واگیر', N'پیشگیری و کنترل بیماری‌های عفونی', 30000000.00, 7, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'برنامه‌های غربالگری', N'غربالگری سرطان و سایر بیماری‌ها', 25000000.00, 7, 2, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به داروخانه‌ها
(N'تأمین داروهای عمومی', N'توزیع داروهای پرمصرف', 40000000.00, 8, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'داروهای تخصصی', N'تأمین داروهای کمیاب و گران قیمت', 80000000.00, 8, 2, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به تجهیزات پزشکی
(N'تجهیزات تشخیصی', N'خرید و نگهداری دستگاه‌های تشخیصی', 90000000.00, 9, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'تجهیزات جراحی', N'ابزار و دستگاه‌های جراحی', 65000000.00, 9, 2, '1234567890', GETDATE(), GETDATE(), 0),

-- عناوین مربوط به کیت‌های تشخیصی
(N'کیت‌های آزمایشگاهی', N'کیت‌های تشخیص سریع بیماری‌ها', 12000000.00, 10, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'تست‌های مولکولی', N'آزمایش‌های ژنتیک و مولکولی', 28000000.00, 10, 2, '1234567890', GETDATE(), GETDATE(), 0);

-- ===============================================
-- 4. ServiceFeatures (ویژگی‌های خدمات)
-- ===============================================

INSERT INTO ServiceFeatures (Name, Description, Code, Icon, Color, DisplayOrder, IsActive, CreateUserId, CreateDate, ModifyDate, IsDelete)
VALUES 
(N'خدمات 24 ساعته', N'ارائه خدمات در تمام ساعات شبانه‌روز', 'SRV_24H', 'clock', '#007bff', 1, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات اورژانسی', N'پوشش خدمات اورژانسی و فوریت‌های پزشکی', 'SRV_EMRG', 'ambulance', '#dc3545', 2, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'تجهیزات پیشرفته', N'استفاده از به‌روزترین تجهیزات پزشکی', 'SRV_TECH', 'cpu', '#28a745', 3, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'پزشکان متخصص', N'حضور پزشکان متخصص و فوق تخصص', 'SRV_SPEC', 'user-md', '#17a2b8', 4, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات بیمه‌ای', N'پوشش بیمه‌های مختلف', 'SRV_INS', 'shield', '#ffc107', 5, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات رایگان', N'ارائه خدمات رایگان به مددجویان', 'SRV_FREE', 'heart', '#fd7e14', 6, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'مراقبت ویژه', N'امکانات مراقبت‌های ویژه و ICU', 'SRV_ICU', 'heartbeat', '#6f42c1', 7, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات تله‌مدیسین', N'ویزیت آنلاین و مشاوره از راه دور', 'SRV_TELE', 'video', '#20c997', 8, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'سیستم نوبت‌دهی', N'نوبت‌دهی آنلاین و هوشمند', 'SRV_APPT', 'calendar', '#6c757d', 9, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'آزمایشگاه مرجع', N'آزمایشگاه با استانداردهای بین‌المللی', 'SRV_LAB', 'flask', '#e83e8c', 10, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'خدمات منزل', N'ارائه خدمات درمانی در منزل', 'SRV_HOME', 'home', '#fd7e14', 11, 1, '1234567890', GETDATE(), GETDATE(), 0),
(N'مراقبت پرستاری', N'مراقبت‌های تخصصی پرستاری', 'SRV_NURSE', 'user-nurse', '#17a2b8', 12, 1, '1234567890', GETDATE(), GETDATE(), 0);

-- ===============================================
-- 5. MainTitleServiceFeatures (ارتباط عناوین و ویژگی‌ها)
-- ===============================================

INSERT INTO MainTitleServiceFeatures (MainTitleId, ServiceFeatureId, IsActive, DisplayOrder, Notes, ActivatedDate, CreateUserId, CreateDate, ModifyDate, IsDelete)
VALUES 
-- ارائه خدمات بستری (MainTitleId=1)
(1, 1, 1, 1, N'خدمات بستری 24 ساعته', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(1, 4, 1, 2, N'پزشکان متخصص در همه بخش‌ها', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(1, 5, 1, 3, N'پوشش تمام بیمه‌ها', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(1, 12, 1, 4, N'مراقبت 24 ساعته پرستاری', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- خدمات جراحی عمومی (MainTitleId=2)
(2, 1, 1, 1, N'اتاق عمل 24 ساعته', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(2, 2, 1, 2, N'جراحی‌های اورژانسی', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(2, 3, 1, 3, N'تجهیزات جراحی مدرن', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(2, 4, 1, 4, N'جراحان متخصص', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- جراحی قلب و عروق (MainTitleId=4)
(4, 3, 1, 1, N'دستگاه‌های پیشرفته قلب', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(4, 4, 1, 2, N'جراحان فوق تخصص قلب', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(4, 7, 1, 3, N'ICU قلب', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(4, 1, 1, 4, N'آماده‌باش 24 ساعته', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- پیوند اعضا (MainTitleId=5)
(5, 3, 1, 1, N'تجهیزات ویژه پیوند', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(5, 4, 1, 2, N'تیم فوق تخصص پیوند', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(5, 7, 1, 3, N'ICU ویژه پیوند', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(5, 1, 1, 4, N'خدمات 24 ساعته پیوند', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- ویزیت تخصصی (MainTitleId=7)
(7, 4, 1, 1, N'متخصصین مختلف', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(7, 8, 1, 2, N'ویزیت آنلاین', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(7, 9, 1, 3, N'نوبت‌دهی آنلاین', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(7, 5, 1, 4, N'پذیرش بیمه', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- خدمات آزمایشگاهی (MainTitleId=8)
(8, 10, 1, 1, N'آزمایشگاه مرجع', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(8, 3, 1, 2, N'دستگاه‌های اتوماتیک', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(8, 9, 1, 3, N'نوبت‌گیری آزمایش', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- مراقبت‌های ویژه (MainTitleId=10)
(10, 7, 1, 1, N'ICU مجهز', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(10, 1, 1, 2, N'مراقبت 24 ساعته', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(10, 4, 1, 3, N'پزشکان ویژه', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(10, 12, 1, 4, N'پرستاری ویژه', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- خدمات آمبولانس (MainTitleId=11)
(11, 2, 1, 1, N'خدمات اورژانسی', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(11, 1, 1, 2, N'آمبولانس 24 ساعته', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(11, 3, 1, 3, N'تجهیزات پیشرفته', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- واکسیناسیون کودکان (MainTitleId=12)
(12, 6, 1, 1, N'واکسن رایگان', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(12, 9, 1, 2, N'نوبت‌دهی واکسن', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(12, 4, 1, 3, N'پزشک کودکان', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- مراقبت مادر و کودک (MainTitleId=14)
(14, 6, 1, 1, N'خدمات رایگان', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(14, 4, 1, 2, N'متخصص زنان', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(14, 11, 1, 3, N'خدمات منزل', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(14, 12, 1, 4, N'مراقبت پرستاری', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- تأمین داروهای عمومی (MainTitleId=17)
(17, 5, 1, 1, N'داروهای بیمه‌ای', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(17, 6, 1, 2, N'دارو رایگان برای نیازمندان', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- داروهای تخصصی (MainTitleId=18)
(18, 5, 1, 1, N'پوشش بیمه‌ای کامل', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(18, 4, 1, 2, N'تجویز متخصص', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),

-- تجهیزات تشخیصی (MainTitleId=19)
(19, 3, 1, 1, N'تجهیزات مدرن', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(19, 4, 1, 2, N'اپراتورهای متخصص', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0),
(19, 9, 1, 3, N'نوبت‌دهی تشخیص', GETDATE(), '1234567890', GETDATE(), GETDATE(), 0);

-- ===============================================
-- نمایش خلاصه داده‌های وارد شده
-- ===============================================

PRINT '===============================================';
PRINT 'خلاصه داده‌های وارد شده:';
PRINT '===============================================';

SELECT 'Departments' as TableName, COUNT(*) as RecordCount FROM Departments WHERE IsDelete = 0
UNION ALL
SELECT 'Scopes', COUNT(*) FROM Scopes WHERE IsDelete = 0
UNION ALL
SELECT 'MainTitles', COUNT(*) FROM MainTitles WHERE IsDelete = 0
UNION ALL
SELECT 'ServiceFeatures', COUNT(*) FROM ServiceFeatures WHERE IsDelete = 0
UNION ALL
SELECT 'MainTitleServiceFeatures', COUNT(*) FROM MainTitleServiceFeatures WHERE IsDelete = 0;

PRINT 'داده‌های نمونه با موفقیت وارد شد!';
PRINT 'حالا می‌توانید API های پروژه را تست کنید.';

-- ===============================================
-- Query های مفید برای تست
-- ===============================================

-- مشاهده ساختار کامل داده‌ها
/*
SELECT 
    d.Name as Department,
    s.Name as Scope, 
    mt.Name as MainTitle,
    mt.Amount,
    sf.Name as ServiceFeature,
    mtsf.IsActive as IsFeatureActive
FROM Departments d
    JOIN Scopes s ON d.Id = s.DepartmentId
    JOIN MainTitles mt ON s.Id = mt.ScopeId  
    LEFT JOIN MainTitleServiceFeatures mtsf ON mt.Id = mtsf.MainTitleId
    LEFT JOIN ServiceFeatures sf ON mtsf.ServiceFeatureId = sf.Id
WHERE d.IsDelete = 0 AND s.IsDelete = 0 AND mt.IsDelete = 0
ORDER BY d.Name, s.Name, mt.DisplayOrder, mtsf.DisplayOrder;
*/

-- مجموع مبالغ هر اداره کل
/*
SELECT 
    d.Name as Department,
    COUNT(mt.Id) as TotalMainTitles,
    SUM(mt.Amount) as TotalAmount,
    AVG(mt.Amount) as AverageAmount
FROM Departments d
    JOIN Scopes s ON d.Id = s.DepartmentId
    JOIN MainTitles mt ON s.Id = mt.ScopeId
WHERE d.IsDelete = 0 AND s.IsDelete = 0 AND mt.IsDelete = 0
GROUP BY d.Id, d.Name
ORDER BY TotalAmount DESC;
*/