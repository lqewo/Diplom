using System;
using System.Collections.Generic;
using System.Globalization;
using Diplom.Common.Models;
using Xamarin.Forms;

namespace MobileWorker
{
    public class ConverterPaymentType : IValueConverter
    {
        private readonly Dictionary<object, string> _paymentTypes = new Dictionary<object, string>
        {
            [PaymentType.Cash] = "Наличными",
            [PaymentType.Card] = "Картой"
        };

        private readonly Dictionary<object, string> _statusTypes = new Dictionary<object, string>
        {
            [StatusType.Processing] = "обрабатывается",
            [StatusType.Completed] = "приготовлен",
            [StatusType.Rejected] = "отклонён",
            [StatusType.Accepted] = "принят"
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is PaymentType)
            {
                return _paymentTypes[value];
            }

            if(value is StatusType)
            {
                return _statusTypes[value];
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}