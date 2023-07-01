IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [BaiLam] (
        [BaiLamId] int NOT NULL IDENTITY,
        [MSSV] nvarchar(max) NULL,
        [HoTen] nvarchar(max) NULL,
        [SoCauDung] int NULL,
        [Diem] real NULL,
        [ThoiGianBatDau] datetime2 NULL,
        [ThoiGianDenHan] datetime2 NULL,
        CONSTRAINT [PK_BaiLam] PRIMARY KEY ([BaiLamId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [GiaoVien] (
        [Id] int NOT NULL IDENTITY,
        [TenDangNhap] nvarchar(20) NOT NULL,
        [MatKhau] nvarchar(50) NOT NULL,
        [HoTen] nvarchar(20) NOT NULL,
        [Email] nvarchar(max) NULL,
        [AnhDaiDien] nvarchar(max) NULL,
        [IsGiaoVien] bit NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_GiaoVien] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [KyKiemTra] (
        [KyKiemTraId] int NOT NULL IDENTITY,
        [TenKyKiemTra] nvarchar(max) NOT NULL,
        [ThoiGianBatDau] datetime2 NOT NULL,
        [ThoiGianKetThuc] datetime2 NOT NULL,
        [ThoiGianLamBai] int NOT NULL,
        CONSTRAINT [PK_KyKiemTra] PRIMARY KEY ([KyKiemTraId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [MonHoc] (
        [MonHocId] int NOT NULL IDENTITY,
        [TenMonHoc] nvarchar(max) NOT NULL,
        [MaMonHoc] nvarchar(max) NOT NULL,
        [MoTa] ntext NOT NULL,
        CONSTRAINT [PK_MonHoc] PRIMARY KEY ([MonHocId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [TaiKhoan] (
        [TaiKhoanId] int NOT NULL IDENTITY,
        [MSSV] nvarchar(20) NOT NULL,
        [MatKhau] nvarchar(50) NOT NULL,
        [Email] nvarchar(max) NULL,
        [HoTen] nvarchar(max) NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_TaiKhoan] PRIMARY KEY ([TaiKhoanId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [De] (
        [DeId] int NOT NULL IDENTITY,
        [KyKiemTraId] int NOT NULL,
        [SoCauHoi] int NOT NULL,
        [DoKhoDe] real NOT NULL,
        CONSTRAINT [PK_De] PRIMARY KEY ([DeId]),
        CONSTRAINT [FK_De_KyKiemTra_KyKiemTraId] FOREIGN KEY ([KyKiemTraId]) REFERENCES [KyKiemTra] ([KyKiemTraId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [Chuong] (
        [ChuongId] int NOT NULL,
        [TenChuong] nvarchar(max) NOT NULL,
        [MonHocId] int NOT NULL,
        CONSTRAINT [PK_Chuong] PRIMARY KEY ([ChuongId]),
        CONSTRAINT [FK_Chuong_MonHoc_MonHocId] FOREIGN KEY ([MonHocId]) REFERENCES [MonHoc] ([MonHocId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [DanhSachThi] (
        [DanhSachThiId] int NOT NULL IDENTITY,
        [TaiKhoanId] int NOT NULL,
        [KyKiemTraId] int NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_DanhSachThi] PRIMARY KEY ([DanhSachThiId]),
        CONSTRAINT [FK_DanhSachThi_KyKiemTra_KyKiemTraId] FOREIGN KEY ([KyKiemTraId]) REFERENCES [KyKiemTra] ([KyKiemTraId]) ON DELETE CASCADE,
        CONSTRAINT [FK_DanhSachThi_TaiKhoan_TaiKhoanId] FOREIGN KEY ([TaiKhoanId]) REFERENCES [TaiKhoan] ([TaiKhoanId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [Bai] (
        [BaiId] int NOT NULL IDENTITY,
        [ChuongId] int NOT NULL,
        [TenBai] nvarchar(max) NOT NULL,
        [SoBai] int NOT NULL,
        [MoTa] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Bai] PRIMARY KEY ([BaiId]),
        CONSTRAINT [FK_Bai_Chuong_ChuongId] FOREIGN KEY ([ChuongId]) REFERENCES [Chuong] ([ChuongId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [CauHoi] (
        [CauHoiId] int NOT NULL IDENTITY,
        [ChuongId] int NOT NULL,
        [NoiDung] nvarchar(max) NULL,
        [DapAnA] nvarchar(max) NULL,
        [DapAnB] nvarchar(max) NULL,
        [DapAnC] nvarchar(max) NULL,
        [DapAnD] nvarchar(max) NULL,
        [DapAnDung] nvarchar(max) NULL,
        [DoKho] real NOT NULL,
        [SoLanLay] int NULL,
        [SoLanTraLoiDung] int NULL,
        CONSTRAINT [PK_CauHoi] PRIMARY KEY ([CauHoiId]),
        CONSTRAINT [FK_CauHoi_Chuong_ChuongId] FOREIGN KEY ([ChuongId]) REFERENCES [Chuong] ([ChuongId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [Muc] (
        [MucId] int NOT NULL IDENTITY,
        [TenMuc] nvarchar(max) NOT NULL,
        [BaiId] int NOT NULL,
        [MucSo] int NOT NULL,
        [NoiDung] ntext NOT NULL,
        CONSTRAINT [PK_Muc] PRIMARY KEY ([MucId]),
        CONSTRAINT [FK_Muc_Bai_BaiId] FOREIGN KEY ([BaiId]) REFERENCES [Bai] ([BaiId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [CauHoi_De] (
        [CauHoi_DeId] int NOT NULL IDENTITY,
        [CauHoiId] int NOT NULL,
        [DeId] int NOT NULL,
        CONSTRAINT [PK_CauHoi_De] PRIMARY KEY ([CauHoi_DeId]),
        CONSTRAINT [FK_CauHoi_De_CauHoi_CauHoiId] FOREIGN KEY ([CauHoiId]) REFERENCES [CauHoi] ([CauHoiId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CauHoi_De_De_DeId] FOREIGN KEY ([DeId]) REFERENCES [De] ([DeId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE TABLE [CauHoi_BaiLam] (
        [CauHoi_BaiLamId] int NOT NULL IDENTITY,
        [BaiLamId] int NOT NULL,
        [CauHoi_DeId] int NOT NULL,
        [DapAnSVChon] nvarchar(max) NULL,
        CONSTRAINT [PK_CauHoi_BaiLam] PRIMARY KEY ([CauHoi_BaiLamId]),
        CONSTRAINT [FK_CauHoi_BaiLam_BaiLam_BaiLamId] FOREIGN KEY ([BaiLamId]) REFERENCES [BaiLam] ([BaiLamId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CauHoi_BaiLam_CauHoi_De_CauHoi_DeId] FOREIGN KEY ([CauHoi_DeId]) REFERENCES [CauHoi_De] ([CauHoi_DeId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_Bai_ChuongId] ON [Bai] ([ChuongId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_CauHoi_ChuongId] ON [CauHoi] ([ChuongId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_CauHoi_BaiLam_BaiLamId] ON [CauHoi_BaiLam] ([BaiLamId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_CauHoi_BaiLam_CauHoi_DeId] ON [CauHoi_BaiLam] ([CauHoi_DeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_CauHoi_De_CauHoiId] ON [CauHoi_De] ([CauHoiId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_CauHoi_De_DeId] ON [CauHoi_De] ([DeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE UNIQUE INDEX [IX_Chuong_ChuongId] ON [Chuong] ([ChuongId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_Chuong_MonHocId] ON [Chuong] ([MonHocId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_DanhSachThi_KyKiemTraId] ON [DanhSachThi] ([KyKiemTraId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_DanhSachThi_TaiKhoanId] ON [DanhSachThi] ([TaiKhoanId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_De_KyKiemTraId] ON [De] ([KyKiemTraId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    CREATE INDEX [IX_Muc_BaiId] ON [Muc] ([BaiId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622142312_baigiangckc')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230622142312_baigiangckc', N'6.0.18');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622191733_baigiangckc1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230622191733_baigiangckc1', N'6.0.18');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230622192054_baigiangckc2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230622192054_baigiangckc2', N'6.0.18');
END;
GO

COMMIT;
GO

