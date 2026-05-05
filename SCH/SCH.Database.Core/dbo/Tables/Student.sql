CREATE TABLE [dbo].[Student] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]    NVARCHAR (400) NOT NULL,
    [LastName]     NVARCHAR (400) NULL,
    [Email]        NVARCHAR (400) NULL,
    [PhoneNumber]  NVARCHAR (20)  NULL,
    [SSN]          NVARCHAR (20)  NULL,
    [Image]        NVARCHAR (400) NULL,
    [StartDate]    DATE           NULL,
    [IsActive]     BIT            NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME2 (7)  NOT NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME2 (7)  NULL,
    [RowVersion]   ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Student_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Student_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Student_ModifiedBy]
    ON [dbo].[Student]([ModifiedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Student_CreatedBy]
    ON [dbo].[Student]([CreatedBy] ASC);

