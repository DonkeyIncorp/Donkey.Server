using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using NLog;

namespace Donkey.Administration.BusinessLogic
{
    internal class EmailSender : BackgroundService, IDisposable
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IOrderManager _orderManager;
        private readonly List<string> _receiversEmails = new List<string>()
        {
            "grachev.alekcandr@mail.ru",
            "neirfy01@yandex.ru"
        };

        public EmailSender(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _orderManager.OrderAdded += OnOrderAdded;
            return Task.CompletedTask;
        }

        private async Task OnOrderAdded(OrderAddedEventArgs arg)
        {
            MimeMessage message = new MimeMessage();

            message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = $"Поступил новый заказ\n{arg.AddedOrder.Name}\n{arg.AddedOrder.Description}"
            };

            message.From.Add(MailboxAddress.Parse("donkey.mailingsystem@gmail.com"));
            message.Subject = "Новый заказ";

            //message.To.Add(MailboxAddress.Parse("grachev.alekcandr@mail.ru"));

            foreach(var receiver in _receiversEmails)
            {
                message.To.Add(MailboxAddress.Parse(receiver));
            }

            await SendMessageAsync(message).ConfigureAwait(false);
        }
        //_masterkey123
        //donkey.mailingsystem@gmail.com

        private async Task SendMessageAsync(MimeMessage message)
        {
            using(var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;

                await client.ConnectAsync("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.Auto).ConfigureAwait(false);

                await client.AuthenticateAsync("donkey.mailingsystem@gmail.com", "_masterkey123").ConfigureAwait(false);

                await client.SendAsync(message).ConfigureAwait(false);

                await client.DisconnectAsync(true).ConfigureAwait(false);

                Logger.Info("Messages has been sent");
            }
        }

        public override void Dispose()
        {
            _orderManager.OrderAdded -= OnOrderAdded;
            base.Dispose();
        }
    }
}
