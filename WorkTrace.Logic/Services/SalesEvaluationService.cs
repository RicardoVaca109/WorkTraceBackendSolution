using MongoDB.Bson;
using WorkTrace.Application.DTOs.SalesEvaluationDTO;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class SalesEvaluationService(ISalesEvaluationRepository salesEvaluationRepository, IFileService fileService) : ISalesEvaluationService
{
    public async Task SubmitEvaluationAsync(CreateSalesEvaluationRequest request, string evaluatorClientId)
    {
        MediaFile? signatureFile = null;

        if (request.ClientSignature != null)
        {
            var url = await fileService.SaveFileAsync(
                request.ClientSignature,
                "sales-signatures");

            signatureFile = new MediaFile
            {
                Url = url,
                UploadedAt = DateTime.UtcNow
            };
        }

        var evaluation = new SalesEvaluation
        {
            TargetUserId = ObjectId.Parse(request.TargetUserId),
            EvaluatorClientId = ObjectId.Parse(evaluatorClientId),
            FormTemplateId = ObjectId.Parse(request.FormType),
            Answers = request.Answers.Select(a => new FormAnswers
            {
                QuestionKey = a.QuestionKey,
                QuestionValue = a.QuestionValue,
                AnswerType = a.AnswerType,
                Answer = a.Answer,
                NumericValue = a.NumericValue
            }).ToList(),
            ClientComment = request.ClientComment,
            ClientSignature = new ClientSignature
            {
                Signature = signatureFile,
                SignedBy = request.SignedBy,
                SignedAt = DateTime.UtcNow
            },
            CreatedAt = DateTime.UtcNow
        };

        await salesEvaluationRepository.CreateAsync(evaluation);
    }
}