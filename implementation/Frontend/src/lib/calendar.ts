import { add, startOfWeek } from "date-fns";
import { get } from "svelte/store";
import { me } from "../stores/associate";
import { makeBackendRequestJson } from "./requestHelpers";
import {
  convertStringsToDates,
  type CreateWorkEventDTO,
  type RequestVacationDTO,
  type UseDateInsteadOfString,
  type WorkEvent,
  type WorkEventDTO,
} from "./types";

export const daysPerWeek = 7;
export const hoursPerDay = 24;
export const minutesPerHour = 60;
export const millisecondsPerDay = 1000 * 60 * 60 * 24;
export const daysPerShortMonth = 30;
export const monthLoadRange = 1;
export const dragGranularity = 30;

const locale = "da-dk";

// Id for events
let nextEventId = 0;
export function getNextEventId() {
  nextEventId++;
  return nextEventId;
}

// Server requests
export async function handleGettingEvents(
  requestPeriod: UseDateInsteadOfString<RequestVacationDTO, "start" | "end">
): Promise<WorkEvent[] | undefined> {
  const result = await makeBackendRequestJson("/Schedule/events", "post", {
    start: requestPeriod.start.toISOString(),
    end: requestPeriod.end.toISOString(),
  });
  return result?.map((e) => convertStringsToDates(e, ["start", "end"]));
}
export async function handleAddingEvent(
  event: CreateWorkEventDTO
): Promise<WorkEventDTO | null> {
  const meValue = get(me);
  if (!meValue) return null;

  const result = await makeBackendRequestJson("/Schedule/associate", "post", {
    createEvent: event,
    associateId: meValue.id,
  });
  return result;
}
// Datasets and generation

export const hourStrings = generateHours();
function generateHours(): { index: number; text: string }[] {
  let hours: { index: number; text: string }[] = [];
  for (let i = 0; i < hoursPerDay; i++) {
    let string = i.toString().padStart(2, "0") + ":00";
    hours.push({ index: i, text: string });
  }
  return hours;
}

// Formatting functions
export const convertEventToTimeString = (eventData: WorkEvent) =>
  `${timeFormatter(eventData.start)} - ${timeFormatter(eventData.end)}`;

export function convertTimeToPercent(time: Date): number {
  const totalMinutes = 60 * 24;
  const timeMinutes = time.getHours() * 60 + time.getMinutes();

  return timeMinutes / totalMinutes;
}
export function convertPercentToTime(percent: number): Date {
  const floatingHour = hoursPerDay * percent;
  const integerHour = Math.floor(floatingHour);
  const floatingMinute = (floatingHour - integerHour) * minutesPerHour;
  const integerMinute = Math.floor(floatingMinute);
  const alignedMinute =
    Math.floor(integerMinute / dragGranularity) * dragGranularity;
  return new Date(1970, 1, 1, integerHour, alignedMinute);
}

export const weekDayFormatter = Intl.DateTimeFormat(locale, {
  weekday: "long",
}).format;

export const monthFormatter = Intl.DateTimeFormat(locale, {
  month: "long",
}).format;

export const timeFormatter = Intl.DateTimeFormat(locale, {
  hour: "numeric",
  minute: "numeric",
}).format;

export const dateFormatter = Intl.DateTimeFormat(locale, {
  dateStyle: "short",
}).format;

export const longDateFormatter = Intl.DateTimeFormat(locale, {
  dateStyle: "long",
}).format;

// Week creation functions
export function getWeekDays(date: Date): Date[] {
  const monday = startOfWeek(date, { weekStartsOn: 1 });

  let dates: Date[] = [];
  for (let i = 0; i < daysPerWeek; i++) {
    dates.push(add(monday, { days: i }));
  }

  return dates;
}

type EventWithColumnIndex = {
  event: WorkEvent;
  columnIndex: number;
};

type EventWithLayout = {
  event: WorkEvent;
  layout: {
    columnSpan: number;
    columnIndex: number;
  };
};

export type DayLayout = {
  events: EventWithLayout[];
  columnCount: number;
};

export let repeatTypes = {
  Day: "Dag",
  Week: "Uge",
  Month: "Måned",
};
export let maxRepeatAmounts: { [key in keyof typeof repeatTypes]: number } = {
  Day: 365,
  Week: 52,
  Month: 12,
};
export type RepeatData = {
  spacing: number;
  type: keyof typeof repeatTypes;
  times: number;
};

// Adapted from: https://stackoverflow.com/a/11323909
export function layoutEventsForDay(events: WorkEvent[]): DayLayout {
  const sortedEvents = [...events].sort(
    (a, b) => Number(a.start) - Number(b.start)
  );

  let maxColumnIndex = 0;

  const eventsWithColumnIndex: EventWithColumnIndex[] = [];
  for (const event of sortedEvents) {
    let existingColumnFitIndex = -1;

    const overlappingEvents = eventsWithColumnIndex.filter((e) =>
      isEventsOverlapping(e.event, event)
    );

    for (let i = 0; i <= maxColumnIndex; i++) {
      const columnIsFree = overlappingEvents.every((e) => e.columnIndex !== i);

      if (columnIsFree) {
        existingColumnFitIndex = i;
        break;
      }
    }

    if (existingColumnFitIndex !== -1) {
      eventsWithColumnIndex.push({
        event,
        columnIndex: existingColumnFitIndex,
      });
    } else {
      maxColumnIndex++;
      eventsWithColumnIndex.push({
        event,
        columnIndex: maxColumnIndex,
      });
    }
  }

  return expandEventsInLayout({
    columnCount: maxColumnIndex + 1,
    events: eventsWithColumnIndex.map(({ event, columnIndex }) => ({
      event,
      layout: { columnIndex, columnSpan: 1 },
    })),
  });
}

function expandEventsInLayout(layout: DayLayout): DayLayout {
  // TODO: Implement expanding events
  return layout;
}

function isEventsOverlapping(a: WorkEvent, b: WorkEvent): boolean {
  return (
    a.id !== b.id &&
    Number(a.end) > Number(b.start) &&
    Number(a.start) < Number(b.end)
  );
}
