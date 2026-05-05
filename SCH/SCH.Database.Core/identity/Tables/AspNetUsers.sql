CREATE TABLE [identity].[AspNetUsers] (
    [Id]                   INT                IDENTITY (1, 1) NOT NULL,
    [FirstName]            NVARCHAR (100)     NOT NULL,
    [LastName]             NVARCHAR (100)     NOT NULL,
    [IsActive]             BIT                NOT NULL,
    [CreatedDate]          DATETIME2 (7)      NOT NULL,
    [LastLoginDate]        DATETIME2 (7)      NULL,
    [CreatedBy]            INT                NULL,
    [ModifiedBy]           INT                NULL,
    [ModifiedDate]         DATETIME2 (7)      NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUsers_AspNetUsers_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [identity].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_AspNetUsers_AspNetUsers_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [identity].[AspNetUsers] ([Id])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [identity].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [identity].[AspNetUsers]([NormalizedEmail] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_ModifiedBy]
    ON [identity].[AspNetUsers]([ModifiedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_CreatedBy]
    ON [identity].[AspNetUsers]([CreatedBy] ASC);

