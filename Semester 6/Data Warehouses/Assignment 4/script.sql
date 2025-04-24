CREATE TABLE Student (
	[id] CHAR(6),
	[name] NVARCHAR(50) NOT NULL,
	[email] VARCHAR(50),
	[birthDate] DATE,
	[status] VARCHAR(9),

	CONSTRAINT pk_student PRIMARY KEY (id)
);


CREATE TABLE Teacher (
	[id] CHAR(5),
	[name] NVARCHAR(50) NOT NULL,
	[email] VARCHAR(50),
	[hireDate] DATE NOT NULL,

	CONSTRAINT pk_teacher PRIMARY KEY (id)
);


CREATE TABLE Course (
	[id] CHAR(4),
	[title] NVARCHAR(50) NOT NULL,
	[credits] INT,
	[teacherId] CHAR(5) NOT NULL,

	CONSTRAINT pk_course PRIMARY KEY (id),
	CONSTRAINT fk_course_teacher FOREIGN KEY (teacherId) REFERENCES Teacher(id)
);


CREATE TABLE Enrollment (
	[id] INT IDENTITY(1,1),
	[date] DATE,
	[grade] DECIMAL(2, 1),
	[studentId] CHAR(6) NOT NULL,
	[courseId] CHAR(4) NOT NULL FOREIGN KEY REFERENCES Course(id),

	CONSTRAINT pk_enrollment PRIMARY KEY (id),
	CONSTRAINT fk_enrollment_student FOREIGN KEY (studentId) REFERENCES Student(id),
	CONSTRAINT fk_enrollment_course FOREIGN KEY (courseId) REFERENCES Course(id)
);


INSERT INTO Student (id, name, email, birthDate, status)
SELECT DISTINCT
	student_id,
	name_x,
	email_x,
	birthdate,
	status
FROM Uczelnia
ORDER BY 1;


INSERT INTO Teacher (id, name, email, hireDate)
SELECT DISTINCT
	teacher_id,
	name_y,
	email_y,
	hire_date
FROM Uczelnia
ORDER BY 1;


INSERT INTO Course (id, title, credits, teacherId)
SELECT DISTINCT
	course_id,
	title,
	credits,
	teacher_id
FROM Uczelnia
ORDER BY 1;


INSERT INTO Enrollment (date, grade, studentId, courseId)
SELECT DISTINCT
	TRY_CONVERT(DATE, enrollment_date),
	TRY_CAST(grade AS DECIMAL(2,1)),
	student_id,
	course_id
FROM Uczelnia
ORDER BY 1;


UPDATE Student
SET email = NULL
WHERE email = 'noemail@';


UPDATE Course
SET credits = NULL
WHERE credits > 10 OR credits < 1;


UPDATE Enrollment
SET grade = 5
WHERE grade > 5;
