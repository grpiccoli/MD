sqlcmd -S 127.0.0.1 -U SA -P 34erdfERDF -Q "DROP DATABASE [aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E]"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "BULK INSERT dbo.AreaCodeProvinces FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/Locality/AreaCodeProvinces.tsv' WITH (DataFileType='widechar')"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "BULK INSERT dbo.AreaCodes FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/Locality/AreaCodes.tsv' WITH (DataFileType='widechar')"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Localities FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/Locality/Localities.tsv' WITH (DataFileType='widechar')"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "SET QUOTED_IDENTIFIER ON; BULK INSERT dbo.Doctors FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/MD/Doctors.tsv' WITH (DataFileType='widechar')"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "BULK INSERT dbo.InsuranceLocations FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/MD/InsuranceLocations.tsv' WITH (DataFileType='widechar')"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "BULK INSERT dbo.MedicalAttentionMediums FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/MD/MedicalAttentionMediums.tsv' WITH (DataFileType='widechar')"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "BULK INSERT dbo.MediumDoctors FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/MD/MediumDoctors.tsv' WITH (DataFileType='widechar')"
sqlcmd -S 127.0.0.1 -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -U SA -P 34erdfERDF -Q "BULK INSERT dbo.Places FROM '/media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/ConsultaMD/Data/MD/Places.tsv' WITH (DataFileType='widechar')"

dotnet user-secrets set "Twilio:VerificationServiceSID" "VAd6494eac9fa82fac38ae6cea0b50e846"
dotnet user-secrets set "Twilio:AuthToken" "837533360893e16b150eca01c03c811f"
dotnet user-secrets set "Twilio:AccountSID" "AC3e87b4b00a0e460583a7e8b77bca6620"

SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
