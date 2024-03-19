import { addHours } from "date-fns";
import { get, writable } from "svelte/store";
import type { RepeatData } from "../lib/calendar";
import type { CreateWorkEvent, Vacation, WorkEvent } from "../lib/types";
import { associateToOption, me, type CustomOption } from "./associate";

export const events = writable<WorkEvent[]>([]);
export const vacationsInEventRange = writable<Vacation[]>([]);
export const creating = writable<{
  creating: boolean;
  newEvent: CreateWorkEvent;
  repeating: boolean;
  repeatData: RepeatData;
  extraOptions: {
    shared: boolean;
    associateScheduleOptions: CustomOption[];
  };
}>({
  creating: false,
  repeating: false,
  repeatData: defaultRepeatData(),
  newEvent: defaultCreateWorkEvent(new Date()),
  extraOptions: defaultExtraOptions(),
});
export const hiddenCalendars = writable<string[]>([]);

function defaultRepeatData(): RepeatData {
  return { spacing: 1, type: "Week", times: 4 };
}
function defaultExtraOptions() {
  const meValue = get(me);
  return {
    shared: false,
    associateScheduleOptions: meValue ? [associateToOption(meValue)] : [],
  };
}

function defaultCreateWorkEvent(startDate: Date): CreateWorkEvent {
  return {
    eventType: "None",
    start: startDate,
    end: addHours(startDate, 1),
    note: null,
    atThePhone: false,
    workingFromHome: false,
  };
}

export function startCreatingEvent(startDate?: Date) {
  editing.update((e) => ({ ...e, editing: false }));

  const date = startDate ?? new Date();

  creating.set({
    creating: true,
    repeating: false,
    repeatData: defaultRepeatData(),
    newEvent: defaultCreateWorkEvent(date),
    extraOptions: defaultExtraOptions(),
  });
}

export function stopCreatingEvent() {
  creating.update((c) => ({
    ...c,
    creating: false,
  }));
}

export const currentDate = writable<Date>(new Date());

export const editing = writable<{
  selectedEvent: WorkEvent | undefined;
  editing: boolean;
}>({ editing: false, selectedEvent: undefined });

export function stopEditing() {
  editing.update((e) => ({ ...e, editing: false }));
}

export function startEditing(eventToEdit: WorkEvent) {
  creating.update((e) => ({ ...e, creating: false }));
  editing.set({ selectedEvent: eventToEdit, editing: true });
}

export function updateEventInStore(updatedEvent: WorkEvent) {
  events.update((events) =>
    events.map((e) => (e.id === updatedEvent.id ? updatedEvent : e))
  );
}

export function deleteEventFromStore(id: string) {
  events.update((events) => events.filter((e) => id != e.id));
}

export function addEventToStore(event: WorkEvent) {
  events.update((events) => [...events, event]);
}

export function toggleHideCalendar(calendarId: string) {
  hiddenCalendars.update((hidden) =>
    hidden.includes(calendarId)
      ? hidden.filter((id) => id !== calendarId)
      : [...hidden, calendarId]
  );
}
