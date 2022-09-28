CREATE TABLE Teachers (
    [teacherid] INT PRIMARY KEY NOT NULL,
    [name] NVARCHAR (40) NOT NULL,
    [joiningdate] DATE NOT NULL,
    [salary] MONEY NOT NULL,
    [address]  NVARCHAR (50) NOT NULL,
    [phone] NVARCHAR (30) NOT NULL,
    [isWorking] BIT NULL,
    [picture] NVARCHAR (150) NOT NULL,
);
GO
CREATE TABLE Projects (
    [projectid]   INT PRIMARY KEY NOT NULL,
    [projectname] NVARCHAR (40) NOT NULL,
    [budget]      MONEY         NOT NULL,
    [isRunning]   BIT           NULL,
    [teacherid]  INT REFERENCES Teachers([teacherid]) NULL,
);