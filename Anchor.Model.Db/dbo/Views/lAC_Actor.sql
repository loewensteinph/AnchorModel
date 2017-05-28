CREATE VIEW [dbo].[lAC_Actor]
WITH SCHEMABINDING
AS
SELECT [AC].AC_ID,
       [AC].Metadata_AC,
       [NAM].AC_NAM_AC_ID,
       [NAM].Metadata_AC_NAM,
       [NAM].AC_NAM_ChangedAt,
       [NAM].AC_NAM_Actor_Name,
       [GEN].AC_GEN_AC_ID,
       [GEN].Metadata_AC_GEN,
       [kGEN].GEN_Gender AS AC_GEN_GEN_Gender,
       [kGEN].Metadata_GEN AS AC_GEN_Metadata_GEN,
       [GEN].AC_GEN_GEN_ID,
       [PLV].AC_PLV_AC_ID,
       [PLV].Metadata_AC_PLV,
       [PLV].AC_PLV_ChangedAt,
       [kPLV].PLV_ProfessionalLevel AS AC_PLV_PLV_ProfessionalLevel,
       [kPLV].Metadata_PLV AS AC_PLV_Metadata_PLV,
       [PLV].AC_PLV_PLV_ID
FROM   [dbo].[AC_Actor] AS [AC]
       LEFT OUTER JOIN
       [dbo].[AC_NAM_Actor_Name] AS [NAM]
       ON [NAM].AC_NAM_AC_ID = [AC].AC_ID
          AND [NAM].AC_NAM_ChangedAt = (SELECT max(sub.AC_NAM_ChangedAt)
                                        FROM   [dbo].[AC_NAM_Actor_Name] AS sub
                                        WHERE  sub.AC_NAM_AC_ID = [AC].AC_ID)
       LEFT OUTER JOIN
       [dbo].[AC_GEN_Actor_Gender] AS [GEN]
       ON [GEN].AC_GEN_AC_ID = [AC].AC_ID
       LEFT OUTER JOIN
       [dbo].[GEN_Gender] AS [kGEN]
       ON [kGEN].GEN_ID = [GEN].AC_GEN_GEN_ID
       LEFT OUTER JOIN
       [dbo].[AC_PLV_Actor_ProfessionalLevel] AS [PLV]
       ON [PLV].AC_PLV_AC_ID = [AC].AC_ID
          AND [PLV].AC_PLV_ChangedAt = (SELECT max(sub.AC_PLV_ChangedAt)
                                        FROM   [dbo].[AC_PLV_Actor_ProfessionalLevel] AS sub
                                        WHERE  sub.AC_PLV_AC_ID = [AC].AC_ID)
       LEFT OUTER JOIN
       [dbo].[PLV_ProfessionalLevel] AS [kPLV]
       ON [kPLV].PLV_ID = [PLV].AC_PLV_PLV_ID;