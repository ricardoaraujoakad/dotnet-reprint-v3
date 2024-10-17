using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ebao.V2.DPEM.Api.Binding.Requests;

public class UpdateProposalStatusRequest
{
    [JsonPropertyName("MessageId")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("ProposalId")]
    public ulong ProposalId { get; set; }

    [JsonPropertyName("Status")]
    public int Status { get; set; }

    [JsonPropertyName("PolicyNumber")]
    public string PolicyNumber { get; set; }

    [JsonPropertyName("BookedAt")]
    public DateTime? BookedAt { get; set; }

    [JsonPropertyName("RunningTime")]
    public TimeSpan RunningTime { get; set; }

    [JsonPropertyName("BookingLogs")]
    public List<BookingLog> BookingLogs { get; set; }
}

public class BookingLog
{
    [JsonPropertyName("LogText")]
    public string LogText { get; set; }

    [JsonPropertyName("LogType")]
    public int LogType { get; set; }

    [JsonPropertyName("LogDate")]
    public DateTime LogDate { get; set; }
}