-- Run with sqlcmd (SQLCMD mode) from the /database directory, e.g.:
--   sqlcmd -S (localdb)\MSSQLLocalDB -d CarRentalDb -i Deploy.sql
:r .\Tables\001_CreateTables.sql
:r .\StoredProcedures\Vehicle.sql
:r .\StoredProcedures\Availability.sql
:r .\StoredProcedures\Location.sql
:r .\StoredProcedures\RentalRate.sql
:r .\StoredProcedures\Customer.sql
:r .\StoredProcedures\Booking.sql
:r .\StoredProcedures\Payment.sql
