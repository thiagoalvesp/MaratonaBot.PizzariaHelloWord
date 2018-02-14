using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MaratonaBot.PizzariaHelloWord.Dialogs
{
    [Serializable]
    public class QnaDialog : QnAMakerDialog
    {
        public QnaDialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnaSubscriptionKey"], ConfigurationManager.AppSettings["QnaKnowledgebaseId"],"Não sei responder essa pergunta!",0.3)))
        {

        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var primeiraResposta = result.Answers.First().Answer;

            Activity resposta = ((Activity)context.Activity).CreateReply();

            var dadosReposta = primeiraResposta.Split(';');

            if (dadosReposta.Length == 1)
            {
                await context.PostAsync(primeiraResposta);
                return;
            }

            var titulo = dadosReposta[0];
            var descricao = dadosReposta[1];
            var url = dadosReposta[2];
            var urlImagem = dadosReposta[3];

            HeroCard card = new HeroCard
            {
                Title = titulo,
                Subtitle = descricao
            };

            card.Buttons = new List<CardAction>{
                new CardAction(ActionTypes.OpenUrl,"Quero ver agora!!!", value:url)
            };

            card.Images = new List<CardImage>
            {
                new CardImage(url = urlImagem)
            };

            resposta.Attachments.Add(card.ToAttachment());
            await context.PostAsync(resposta);
        }
    }
}