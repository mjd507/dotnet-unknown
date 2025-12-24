using DotNetUnknown.Http.resp;

namespace DotNetUnknown.Http.clients;

public class StackoverflowHttpClient(HttpClient httpClient)
{
    public Task<QuestionContainer?> GetQuestions(string tagged, string sort)
    {
        return httpClient.GetFromJsonAsync<QuestionContainer>(
            $"/questions?site=stackoverflow&tagged={tagged}&sorts={sort}");
    }
}