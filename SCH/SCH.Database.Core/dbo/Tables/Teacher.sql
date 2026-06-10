CREATE TABLE [dbo].[Teacher] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (400) NOT NULL,
    [UserId]       INT            NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME2 (7)  NOT NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME2 (7)  NULL,
    [RowVersion]   ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Teacher] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Teacher_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Teacher_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Teacher_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE SET NULL
);






GO
CREATE NONCLUSTERED INDEX [IX_Teacher_ModifiedBy]
    ON [dbo].[Teacher]([ModifiedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Teacher_CreatedBy]
    ON [dbo].[Teacher]([CreatedBy] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Teacher_UserId]
    ON [dbo].[Teacher]([UserId] ASC) WHERE ([UserId] IS NOT NULL);

