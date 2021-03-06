USE [hrmplDB]
GO
/****** Object:  UserDefinedFunction [dbo].[sf_AddTime]    Script Date: 30/11/2017 4:27:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[sf_AddTime]
(
	-- Add the parameters for the function here
	 @RequestedTime TIME(0),
        @TimeIntervel TIME(0)
)
RETURNS time
AS
BEGIN
	-- Declare the return variable here
	declare @result time;
	SELECT @result= DATEADD(SECOND,DATEDIFF(SECOND,'00:00:00',@RequestedTime),@TimeIntervel);

	-- Return the result of the function
	RETURN @result

END
