
--UPDATE LiftResults
--SET
--    Total = CASE
--                WHEN Total IS NOT NULL THEN ROUND(Total * 2, 0) / 2.0
--                ELSE Total
--            END,
--    Squat = CASE
--                WHEN Squat IS NOT NULL THEN ROUND(Squat * 2, 0) / 2.0
--                ELSE Squat
--            END,
--    Bench = CASE
--                WHEN Bench IS NOT NULL THEN ROUND(Bench * 2, 0) / 2.0
--                ELSE Bench
--            END,
--    Deadlift = CASE
--                WHEN Deadlift IS NOT NULL THEN ROUND(Deadlift * 2, 0) / 2.0
--                ELSE Deadlift
--            END
--WHERE 
--    ABS(Total - FLOOR(Total) - 0.99) < 0.001 OR 
--    ABS(Total - FLOOR(Total) - 0.98) < 0.001 OR 
--    ABS(Total - FLOOR(Total) - 0.49) < 0.001 OR 
--    ABS(Total - FLOOR(Total) - 0.02) < 0.001 OR 
--    ABS(Total - FLOOR(Total) - 0.01) < 0.001 OR 
--    ABS(Total - FLOOR(Total) - 0.48) < 0.001 OR 
--    ABS(Total - FLOOR(Total) - 0.92) < 0.001 OR
--    ABS(Total - FLOOR(Total) - 0.93) < 0.001 OR

--    ABS(Squat - FLOOR(Squat) - 0.99) < 0.001 OR 
--    ABS(Squat - FLOOR(Squat) - 0.98) < 0.001 OR 
--    ABS(Squat - FLOOR(Squat) - 0.49) < 0.001 OR 
--    ABS(Squat - FLOOR(Squat) - 0.02) < 0.001 OR 
--    ABS(Squat - FLOOR(Squat) - 0.01) < 0.001 OR 
--    ABS(Squat - FLOOR(Squat) - 0.48) < 0.001 OR 
--    ABS(Squat - FLOOR(Squat) - 0.92) < 0.001 OR
--    ABS(Squat - FLOOR(Squat) - 0.93) < 0.001 OR

--    ABS(Bench - FLOOR(Bench) - 0.99) < 0.001 OR 
--    ABS(Bench - FLOOR(Bench) - 0.98) < 0.001 OR 
--    ABS(Bench - FLOOR(Bench) - 0.49) < 0.001 OR 
--    ABS(Bench - FLOOR(Bench) - 0.02) < 0.001 OR 
--    ABS(Bench - FLOOR(Bench) - 0.01) < 0.001 OR 
--    ABS(Bench - FLOOR(Bench) - 0.48) < 0.001 OR 
--    ABS(Bench - FLOOR(Bench) - 0.92) < 0.001 OR
--    ABS(Bench - FLOOR(Bench) - 0.93) < 0.001 OR

--    ABS(Deadlift - FLOOR(Deadlift) - 0.99) < 0.001 OR 
--    ABS(Deadlift - FLOOR(Deadlift) - 0.98) < 0.001 OR 
--    ABS(Deadlift - FLOOR(Deadlift) - 0.49) < 0.001 OR 
--    ABS(Deadlift - FLOOR(Deadlift) - 0.02) < 0.001 OR 
--    ABS(Deadlift - FLOOR(Deadlift) - 0.01) < 0.001 OR 
--    ABS(Deadlift - FLOOR(Deadlift) - 0.48) < 0.001 OR 
--    ABS(Deadlift - FLOOR(Deadlift) - 0.93) < 0.001;

CREATE OR REPLACE PROCEDURE public."FIX_ROUND_RESULT"()
    LANGUAGE 'plpgsql'
    
AS $BODY$
BEGIN

    UPDATE "LiftResults"
    SET
        "Total" = CASE
                    WHEN "Total" IS NOT NULL AND ("Total" - FLOOR("Total")) NOT IN (0.0, 0.5) THEN round_nearest_2_5("Total")
                    ELSE "Total"
                  END,
        "Squat" = CASE
                    WHEN "Squat" IS NOT NULL AND ("Squat" - FLOOR("Squat")) NOT IN (0.0, 0.5) THEN round_nearest_2_5("Squat")
                    ELSE "Squat"
                  END,
        "Bench" = CASE
                    WHEN "Bench" IS NOT NULL AND ("Bench" - FLOOR("Bench")) NOT IN (0.0, 0.5) THEN round_nearest_2_5("Bench")
                    ELSE "Bench"
                  END,
        "Deadlift" = CASE
                      WHEN "Deadlift" IS NOT NULL AND ("Deadlift" - FLOOR("Deadlift")) NOT IN (0.0, 0.5) THEN round_nearest_2_5("Deadlift")
                      ELSE "Deadlift"
                    END
    WHERE 
        ("Total" - FLOOR("Total")) NOT IN (0.0, 0.5) OR
        ("Squat" - FLOOR("Squat")) NOT IN (0.0, 0.5) OR
        ("Bench" - FLOOR("Bench")) NOT IN (0.0, 0.5) OR
        ("Deadlift" - FLOOR("Deadlift")) NOT IN (0.0, 0.5);


END;
$BODY$;

CREATE OR REPLACE FUNCTION round_nearest_2_5(x double precision)
RETURNS double precision AS $$
BEGIN
    RETURN ROUND((x / 2.5)::numeric, 0) * 2.5;
END;
$$ LANGUAGE plpgsql IMMUTABLE;




GO

