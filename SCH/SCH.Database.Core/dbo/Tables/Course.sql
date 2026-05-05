CREATE TABLE [dbo].[Course] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (400) NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME2 (7)  NOT NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME2 (7)  NULL,
    [RowVersion]   ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Course_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Course_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Course_ModifiedBy]
    ON [dbo].[Course]([ModifiedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Course_CreatedBy]
    ON [dbo].[Course]([CreatedBy] ASC);

