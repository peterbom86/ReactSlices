﻿CREATE PROCEDURE rdh_ImportSAPUOM_TotalList_Sirius

AS

INSERT INTO SAP_UOM_TotalList ( Material, Unit, Pcs_Ren, Pcs_Rez, Ean, FK_FileImportID )
SELECT MATERIAL, UNIT, PCS_REN, PCS_REZ, EAN, PK_FileImportID
FROM SAP_FileImport FI
  INNER JOIN SAP_UOM UOM ON PK_FileImportID = FK_FileImportID
WHERE IsHandled = 0 AND 
  NOT EXISTS 
    (SELECT * FROM SAP_UOM_TotalList TL
     WHERE UOM.MATERIAL = TL.Material AND UOM.UNIT = TL.Unit)

DELETE FROM TL
FROM SAP_UOM_TotalList TL
WHERE Material IN 
  (SELECT MATERIAL FROM SAP_FileImport FI
     INNER JOIN SAP_UOM UOM ON PK_FileImportID = UOM.FK_FileImportID
   WHERE IsHandled = 0) AND
  NOT EXISTS 
    (SELECT * FROM SAP_FileImport FI
       INNER JOIN SAP_UOM UOM ON PK_FileImportID = UOM.FK_FileImportID
     WHERE IsHandled = 0 AND UOM.MATERIAL = TL.Material AND UOM.UNIT = TL.Unit)

UPDATE TL
SET Pcs_Rez = UOM.PCS_REZ,
  Pcs_Ren = UOM.PCS_REN,
  Ean = UOM.EAN
FROM SAP_FileImport FI
  INNER JOIN SAP_UOM UOM ON PK_FileImportID = UOM.FK_FileImportID
  INNER JOIN SAP_UOM_TotalList TL ON UOM.MATERIAL = TL.Material AND UOM.UNIT = TL.Unit
WHERE IsHandled = 0 AND (UOM.PCS_REZ <> TL.Pcs_Rez OR UOM.PCS_REN <> TL.Pcs_Ren OR UOM.EAN <> TL.Ean)

DELETE FROM TL
FROM SAP_UOM_TotalList TL
WHERE FK_FileImportID NOT IN (
  SELECT MAX(FK_FileImportID) FileImportID
  FROM SAP_UOM_TotalList
  GROUP BY Material, Unit)

