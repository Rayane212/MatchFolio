use MatchFolio

CREATE TRIGGER tr_Users_Update
ON Users
AFTER UPDATE
AS
BEGIN
    UPDATE Users
    SET updatedAt = GETDATE()
    WHERE id IN (SELECT DISTINCT id FROM Inserted)
END;