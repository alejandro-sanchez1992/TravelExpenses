using System;
using TravelExpenses.Common.Models;

namespace TravelExpenses.Web.Interfaces
{
	public interface IMailHelper
	{
		Response SendMail(string to, string subject, string body);
	}
}
