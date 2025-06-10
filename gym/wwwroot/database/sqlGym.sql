/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     10/06/2025 12:07:11 SA                       */
/*==============================================================*/

CREATE DATABASE GYM
GO
USE GYM
GO

-- Drop foreign key constraints
IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPakage') AND o.name = 'FK_MEMBERPA_MEMBERPAK_MEMBER')
    ALTER TABLE MemberPakage
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAK_MEMBER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPakage') AND o.name = 'FK_MEMBERPA_MEMBERPAK_PACKAGE')
    ALTER TABLE MemberPakage
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAK_PACKAGE
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPayment') AND o.name = 'FK_MEMBERPA_MEMBERPAY_MEMBER')
    ALTER TABLE MemberPayment
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAY_MEMBER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPayment') AND o.name = 'FK_MEMBERPA_MEMBERPAY_PAYMENT')
    ALTER TABLE MemberPayment
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAY_PAYMENT
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('MemberPayment') AND o.name = 'FK_MEMBERPA_MEMBERPAY_STAFF')
    ALTER TABLE MemberPayment
        DROP CONSTRAINT FK_MEMBERPA_MEMBERPAY_STAFF
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('TrainingSchedule') AND o.name = 'FK_TRAINING_TRAININGS_TRAINER')
    ALTER TABLE TrainingSchedule
        DROP CONSTRAINT FK_TRAINING_TRAININGS_TRAINER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('TrainingSchedule') AND o.name = 'FK_TRAINING_TRAININGS_MEMBER')
    ALTER TABLE TrainingSchedule
        DROP CONSTRAINT FK_TRAINING_TRAININGS_MEMBER
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('"User"') AND o.name = 'FK_USER_INCLUDE_ROLE')
    ALTER TABLE "User"
        DROP CONSTRAINT FK_USER_INCLUDE_ROLE
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('UserNotification') AND o.name = 'FK_USERNOTI_USERNOTIF_NOTIFICA')
    ALTER TABLE UserNotification
        DROP CONSTRAINT FK_USERNOTI_USERNOTIF_NOTIFICA
GO

IF EXISTS (SELECT 1
           FROM sys.sysreferences r JOIN sys.sysobjects o ON (o.id = r.constid AND o.type = 'F')
           WHERE r.fkeyid = OBJECT_ID('UserNotification') AND o.name = 'FK_USERNOTI_USERNOTIF_USER')
    ALTER TABLE UserNotification
        DROP CONSTRAINT FK_USERNOTI_USERNOTIF_USER
GO

-- Drop tables
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Member') AND type = 'U')
    DROP TABLE Member
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPakage') AND name = 'MemberPakage2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPakage.MemberPakage2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPakage') AND name = 'MemberPakage_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPakage.MemberPakage_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('MemberPakage') AND type = 'U')
    DROP TABLE MemberPakage
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPayment') AND name = 'MemberPayment3_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPayment.MemberPayment3_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPayment') AND name = 'MemberPayment2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPayment.MemberPayment2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('MemberPayment') AND name = 'MemberPayment_FK' AND indid > 0 AND indid < 255)
    DROP INDEX MemberPayment.MemberPayment_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('MemberPayment') AND type = 'U')
    DROP TABLE MemberPayment
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Notification') AND type = 'U')
    DROP TABLE Notification
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Package') AND type = 'U')
    DROP TABLE Package
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Payment') AND type = 'U')
    DROP TABLE Payment
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Role') AND type = 'U')
    DROP TABLE Role
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Staff') AND type = 'U')
    DROP TABLE Staff
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('Trainer') AND type = 'U')
    DROP TABLE Trainer
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('TrainingSchedule') AND name = 'TrainingSchedule2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX TrainingSchedule.TrainingSchedule2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('TrainingSchedule') AND name = 'TrainingSchedule_FK' AND indid > 0 AND indid < 255)
    DROP INDEX TrainingSchedule.TrainingSchedule_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('TrainingSchedule') AND type = 'U')
    DROP TABLE TrainingSchedule
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('"User"') AND name = 'include_FK' AND indid > 0 AND indid < 255)
    DROP INDEX "User".include_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('"User"') AND type = 'U')
    DROP TABLE "User"
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('UserNotification') AND name = 'UserNotification2_FK' AND indid > 0 AND indid < 255)
    DROP INDEX UserNotification.UserNotification2_FK
GO

IF EXISTS (SELECT 1 FROM sysindexes WHERE id = OBJECT_ID('UserNotification') AND name = 'UserNotification_FK' AND indid > 0 AND indid < 255)
    DROP INDEX UserNotification.UserNotification_FK
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('UserNotification') AND type = 'U')
    DROP TABLE UserNotification
GO

/*==============================================================*/
/* Table: Member                                                */
/*==============================================================*/
CREATE TABLE Member (
    memberId INT NOT NULL IDENTITY(1,1),
    fullName TEXT NULL,
    dateOfBirth DATETIME NULL,
    sex BIT NULL,
    phone TEXT NULL,
    address TEXT NULL,
    createDate DATETIME NULL,
    CONSTRAINT PK_MEMBER PRIMARY KEY NONCLUSTERED (memberId)
)
GO

/*==============================================================*/
/* Table: MemberPakage                                          */
/*==============================================================*/
CREATE TABLE MemberPakage (
    memberId INT NOT NULL,
    packageId INT NOT NULL,
    startDate DATETIME NULL,
    endDate DATETIME NULL,
    isPaid BIT NULL,
    isActive BIT NULL,
    CONSTRAINT PK_MEMBERPAKAGE PRIMARY KEY (memberId, packageId)
)
GO

/*==============================================================*/
/* Index: MemberPakage_FK                                       */
/*==============================================================*/
CREATE INDEX MemberPakage_FK ON MemberPakage (
    memberId ASC
)
GO

/*==============================================================*/
/* Index: MemberPakage2_FK                                      */
/*==============================================================*/
CREATE INDEX MemberPakage2_FK ON MemberPakage (
    packageId ASC
)
GO

/*==============================================================*/
/* Table: MemberPayment                                         */
/*==============================================================*/
CREATE TABLE MemberPayment (
    memberId INT NOT NULL,
    paymentId INT NOT NULL,
    staffId INT NOT NULL,
    paymentDate DATETIME NULL,
    CONSTRAINT PK_MEMBERPAYMENT PRIMARY KEY (memberId, paymentId, staffId)
)
GO

/*==============================================================*/
/* Index: MemberPayment_FK                                      */
/*==============================================================*/
CREATE INDEX MemberPayment_FK ON MemberPayment (
    memberId ASC
)
GO

/*==============================================================*/
/* Index: MemberPayment2_FK                                     */
/*==============================================================*/
CREATE INDEX MemberPayment2_FK ON MemberPayment (
    paymentId ASC
)
GO

/*==============================================================*/
/* Index: MemberPayment3_FK                                     */
/*==============================================================*/
CREATE INDEX MemberPayment3_FK ON MemberPayment (
    staffId ASC
)
GO

/*==============================================================*/
/* Table: Notification                                          */
/*==============================================================*/
CREATE TABLE Notification (
    notificationId INT NOT NULL IDENTITY(1,1),
    title TEXT NULL,
    content TEXT NULL,
    createdAt DATETIME NULL,
    sendRole TEXT NULL,
    CONSTRAINT PK_NOTIFICATION PRIMARY KEY NONCLUSTERED (notificationId)
)
GO

/*==============================================================*/
/* Table: Package                                               */
/*==============================================================*/
CREATE TABLE Package (
    packageId INT NOT NULL IDENTITY(1,1),
    name TEXT NULL,
    type TEXT NULL,
    price DECIMAL(10,2) NULL,
    durationInDays INT NULL,
    description TEXT NULL,
    CONSTRAINT PK_PACKAGE PRIMARY KEY NONCLUSTERED (packageId)
)
GO

/*==============================================================*/
/* Table: Payment                                               */
/*==============================================================*/
CREATE TABLE Payment (
    paymentId INT NOT NULL IDENTITY(1,1),
    amount DECIMAL(10,2) NULL,
    method TEXT NULL,
    CONSTRAINT PK_PAYMENT PRIMARY KEY NONCLUSTERED (paymentId)
)
GO

/*==============================================================*/
/* Table: Role                                                  */
/*==============================================================*/
CREATE TABLE Role (
    roleId INT NOT NULL IDENTITY(1,1),
    roleName TEXT NULL,
    description TEXT NULL,
    CONSTRAINT PK_ROLE PRIMARY KEY NONCLUSTERED (roleId)
)
GO

/*==============================================================*/
/* Table: Staff                                                 */
/*==============================================================*/
CREATE TABLE Staff (
    staffId INT NOT NULL IDENTITY(1,1),
    fullName TEXT NULL,
    phone TEXT NULL,
    email TEXT NULL,
    workingSince DATETIME NULL,
    CONSTRAINT PK_STAFF PRIMARY KEY NONCLUSTERED (staffId)
)
GO

/*==============================================================*/
/* Table: Trainer                                               */
/*==============================================================*/
CREATE TABLE Trainer (
    trainerId INT NOT NULL IDENTITY(1,1),
    fullName TEXT NULL,
    phone TEXT NULL,
    specialty TEXT NULL,
    scheduleNote TEXT NULL,
    CONSTRAINT PK_TRAINER PRIMARY KEY NONCLUSTERED (trainerId)
)
GO

/*==============================================================*/
/* Table: TrainingSchedule                                      */
/*==============================================================*/
CREATE TABLE TrainingSchedule (
    trainerId INT NOT NULL,
    memberId INT NOT NULL,
    trainingDate DATETIME NULL,
    startTime DATETIME NULL,
    endTime DATETIME NULL,
    node TEXT NULL,
    CONSTRAINT PK_TRAININGSCHEDULE PRIMARY KEY (trainerId, memberId)
)
GO

/*==============================================================*/
/* Index: TrainingSchedule_FK                                   */
/*==============================================================*/
CREATE INDEX TrainingSchedule_FK ON TrainingSchedule (
    trainerId ASC
)
GO

/*==============================================================*/
/* Index: TreatmentSchedule2_FK                                  */
/*==============================================================*/
CREATE INDEX TrainingSchedule2_FK ON TrainingSchedule (
    memberId ASC
)
GO

/*==============================================================*/
/* Table: "User"                                                */
/*==============================================================*/
CREATE TABLE "User" (
    userId INT NOT NULL IDENTITY(1,1),
    roleId INT NOT NULL,
    userName TEXT NULL,
    password TEXT NULL,
    email TEXT NULL,
    referenceId INT NULL,
    status CHAR(10) NULL,
    isAtive BIT NULL,
    CONSTRAINT PK_USER PRIMARY KEY NONCLUSTERED (userId)
)
GO

/*==============================================================*/
/* Index: include_FK                                            */
/*==============================================================*/
CREATE INDEX include_FK ON "User" (
    roleId ASC
)
GO

/*==============================================================*/
/* Table: UserNotification                                      */
/*==============================================================*/
CREATE TABLE UserNotification (
    notificationId INT NOT NULL,
    userId INT NOT NULL,
    timeSend DATETIME NULL,
    seen BIT NULL,
    CONSTRAINT PK_USERNOTIFICATION PRIMARY KEY (notificationId, userId)
)
GO

/*==============================================================*/
/* Index: UserNotification_FK                                   */
/*==============================================================*/
CREATE INDEX UserNotification_FK ON UserNotification (
    notificationId ASC
)
GO

/*==============================================================*/
/* Index: UserNotification2_FK                                  */
/*==============================================================*/
CREATE INDEX UserNotification2_FK ON UserNotification (
    userId ASC
)
GO

-- Add foreign key constraints
ALTER TABLE MemberPakage
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAK_MEMBER FOREIGN KEY (memberId)
        REFERENCES Member (memberId)
GO

ALTER TABLE MemberPakage
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAK_PACKAGE FOREIGN KEY (packageId)
        REFERENCES Package (packageId)
GO

ALTER TABLE MemberPayment
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAY_MEMBER FOREIGN KEY (memberId)
        REFERENCES Member (memberId)
GO

ALTER TABLE MemberPayment
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAY_PAYMENT FOREIGN KEY (paymentId)
        REFERENCES Payment (paymentId)
GO

ALTER TABLE MemberPayment
    ADD CONSTRAINT FK_MEMBERPA_MEMBERPAY_STAFF FOREIGN KEY (staffId)
        REFERENCES Staff (staffId)
GO

ALTER TABLE TrainingSchedule
    ADD CONSTRAINT FK_TRAINING_TRAININGS_TRAINER FOREIGN KEY (trainerId)
        REFERENCES Trainer (trainerId)
GO

ALTER TABLE TrainingSchedule
    ADD CONSTRAINT FK_TRAINING_TRAININGS_MEMBER FOREIGN KEY (memberId)
        REFERENCES Member (memberId)
GO

ALTER TABLE "User"
    ADD CONSTRAINT FK_USER_INCLUDE_ROLE FOREIGN KEY (roleId)
        REFERENCES Role (roleId)
GO

ALTER TABLE UserNotification
    ADD CONSTRAINT FK_USERNOTI_USERNOTIF_NOTIFICA FOREIGN KEY (notificationId)
        REFERENCES Notification (notificationId)
GO

ALTER TABLE UserNotification
    ADD CONSTRAINT FK_USERNOTI_USERNOTIF_USER FOREIGN KEY (userId)
        REFERENCES "User" (userId)
GO

/*==============================================================*/
/*                            INSERT                            */
/*==============================================================*/


INSERT INTO [GYM].[dbo].[Role] ([roleName], [description])
VALUES 
    ('Admin', 'Người quản trị hệ thống, có toàn quyền quản lý'),
    ('Member', 'Hội viên sử dụng dịch vụ phòng gym'),
    ('Staff', 'Nhân viên hỗ trợ và chăm sóc khách hàng'),
    ('Trainer', 'Huấn luyện viên hướng dẫn hội viên');
