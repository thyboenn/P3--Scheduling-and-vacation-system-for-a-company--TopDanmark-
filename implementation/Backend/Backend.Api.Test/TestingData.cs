using System;
using Backend.Api.Messages;
using Backend.Api.Models;

namespace Backend.Api.Test;

public static class TestingData
{
  public static readonly CreateAssociateDTO CreateAssociateDto1 =
    new("jens@example.com", "Jens");

  public static readonly CreateAssociateDTO CreateAssociateDto2 =
    new("carl@example.com", "Carl");

  public static readonly CreateWorkEventDTO CreateWorkEventDto1 =
    new(
      start: new DateTime(2020, 10, 10, hour: 8, 0, 0),
      end: new DateTime(2020, 10, 10, hour: 12, 0, 0),
      eventType: EventType.None,
      workingFromHome: false,
      atThePhone: false,
      note: null
    );

  public static readonly CreateWorkEventDTO CreateWorkEventDto2 =
    new(
      start: new DateTime(2020, 10, 10, hour: 13, 0, 0),
      end: new DateTime(2020, 10, 10, hour: 14, 0, 0),
      eventType: EventType.None,
      workingFromHome: false,
      atThePhone: false,
      note: "This is a note"
    );
}
