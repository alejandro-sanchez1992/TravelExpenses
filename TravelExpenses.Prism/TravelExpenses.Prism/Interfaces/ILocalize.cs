using System;
using System.Globalization;

namespace TravelExpenses.Prism.Interfaces
{
	public interface ILocalize
	{
		CultureInfo GetCurrentCultureInfo();

		void SetLocale(CultureInfo ci);
	}
}
