CREATE TABLE [dbo].[User] (
    [Id]           INT            NOT NULL,
    [FirstName]    NVARCHAR (100) NOT NULL,
    [LastName]     NVARCHAR (100) NOT NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME2 (7)  NOT NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME2 (7)  NULL,
    [RowVersion]   ROWVERSION     NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_User_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_User_AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [identity].[AspNetUsers] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_User_ModifiedBy]
    ON [dbo].[User]([ModifiedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_User_CreatedBy]
    ON [dbo].[User]([CreatedBy] ASC);

