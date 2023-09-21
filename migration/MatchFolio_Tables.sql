create database MatchFolio 
use MatchFolio

DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS ProjectTechnologies;
DROP TABLE IF EXISTS AuthProvider;
DROP TABLE IF EXISTS Experiences;
DROP TABLE IF EXISTS Studies;
DROP TABLE IF EXISTS Skills;
DROP TABLE IF EXISTS TechTypes;
DROP TABLE IF EXISTS TechCategories;
DROP TABLE IF EXISTS Technologies;
DROP TABLE IF EXISTS Projects;
DROP TABLE IF EXISTS Addresses;
DROP TABLE IF EXISTS Users;

CREATE TABLE Users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(max) NOT NULL,
    firstName VARCHAR(50),
    lastName VARCHAR(50),
	birthday DATE DEFAULT NULL, 
    email VARCHAR(100) UNIQUE NOT NULL,
    phoneNumber VARCHAR(20),
	userType BIT,  -- Booléen : Soit il est en recherche d'emplois (userType = 0) soit c'est un Recruteur (userType = 1)
 	cvLink VARCHAR(max),
	linkedinLink VARCHAR(max),
	XLink VARCHAR(max), 
	githubLink VARCHAR(max),
    createdAt DATETIME DEFAULT GETDATE(),
    updatedAt DATETIME
);

CREATE TABLE Addresses (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    street VARCHAR(max),
    city VARCHAR(max),
    state VARCHAR(max),
    postalCode VARCHAR(10),
    country VARCHAR(max)
);

CREATE TABLE Reviews (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    rating INT,
    comment VARCHAR(MAX),
    createdAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Projects (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(MAX),
    linkGit VARCHAR(max),
    linkWeb VARCHAR(max)
);

CREATE TABLE TechTypes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    typeName VARCHAR(50) NOT NULL
);

CREATE TABLE TechCategories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    categoryName VARCHAR(50) NOT NULL
);

CREATE TABLE Technologies (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    logoLink VARCHAR(max),
    officialWebsite VARCHAR(max),
    typeId INT,
    categoryId INT
);

CREATE TABLE ProjectTechnologies (
    projectId INT,
    technologyId INT,
	PRIMARY KEY (projectId, technologyId)
);

CREATE TABLE Skills (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    name VARCHAR(50) NOT NULL,
    level INT
);

CREATE TABLE Experiences (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    jobTitle VARCHAR(100) NOT NULL,
    company VARCHAR(100) NOT NULL,
	description VARCHAR(MAX),
    startDate DATE,
    endDate DATE
);

CREATE TABLE Studies (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    degree VARCHAR(100) NOT NULL,
    institution VARCHAR(100) NOT NULL,
	description VARCHAR(MAX),
    startDate DATE,
    endDate DATE
);

CREATE TABLE AuthProvider (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    name VARCHAR(100) NOT NULL,
    providerId VARCHAR(max) NOT NULL
);

ALTER TABLE Addresses ADD CONSTRAINT FK_Addresses_Users FOREIGN KEY (userId) REFERENCES Users(id);
ALTER TABLE Reviews ADD CONSTRAINT FK_Reviews_Users FOREIGN KEY (userId) REFERENCES Users(id);
ALTER TABLE Projects ADD CONSTRAINT FK_Projects_Users FOREIGN KEY (userId) REFERENCES Users(id);
ALTER TABLE ProjectTechnologies ADD CONSTRAINT FK_ProjectTechnologies_Projects FOREIGN KEY (projectId) REFERENCES Projects(id);
ALTER TABLE ProjectTechnologies ADD CONSTRAINT FK_ProjectTechnologies_Technologies FOREIGN KEY (technologyId) REFERENCES Technologies(id);
ALTER TABLE Technologies
ADD FOREIGN KEY (typeId) REFERENCES TechTypes(id),
    FOREIGN KEY (categoryId) REFERENCES TechCategories(id);
ALTER TABLE Skills ADD CONSTRAINT FK_Skills_Users FOREIGN KEY (userId) REFERENCES Users(id);
ALTER TABLE Experiences ADD CONSTRAINT FK_Experiences_Users FOREIGN KEY (userId) REFERENCES Users(id);
ALTER TABLE Studies ADD CONSTRAINT FK_Studies_Users FOREIGN KEY (userId) REFERENCES Users(id);
ALTER TABLE AuthProvider ADD CONSTRAINT FK_AuthProvider_Users FOREIGN KEY (userId) REFERENCES Users(id);
