using DotNetUnknown.Http.resp;

namespace DotNetUnknown.Http.clients;

public class GitHubHttpClient(HttpClient httpClient)
{
    public Task<List<Milestone>?> GetMilestones(string owner, string repo)
    {
        return httpClient.GetFromJsonAsync<List<Milestone>>($"/repos/{owner}/{repo}/milestones");
    }

    public Task<List<Issue>?> GetIssues(string owner, string repo, string status)
    {
        return httpClient.GetFromJsonAsync<List<Issue>>($"/repos/{owner}/{repo}/issues?status={status}");
    }
}