CREATE TABLE [identity].[AspNetRoles] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Description]      NVARCHAR (255) NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedDate]      DATETIME2 (7)  NOT NULL,
    [Name]             NVARCHAR (256) NULL,
    [NormalizedName]   NVARCHAR (256) NULL,
    [ConcurrencyStamp] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [identity].[AspNetRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);

