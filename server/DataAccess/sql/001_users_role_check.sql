-- Restricts Users.Role to the values of the UserRole enum (DataAccess/Enums.cs).
-- Not applied automatically. Run it once against the database, then re-run scaffold.sh if needed.
-- A CHECK constraint is used instead of a Postgres enum so the scaffolded model keeps mapping Role to string.

-- 1. Make sure no existing row would violate the constraint (should return 0 rows).
SELECT "UserId", "Role" FROM "Users" WHERE "Role" NOT IN ('Admin', 'Player');

-- 2. Add the constraint.
ALTER TABLE "Users"
    ADD CONSTRAINT users_role_check CHECK ("Role" IN ('Admin', 'Player'));
