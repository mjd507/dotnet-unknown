using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Kafka;

[ApiController]
[Route("kafka")]
public sealed class KafkaController(KafkaService kafkaService) : ControllerBase
{
    [HttpGet]
    [Route("send")]
    public async Task<IActionResult> Send()
    {
        await kafkaService.Send("test-message");

        return Ok("send success");
    }
}