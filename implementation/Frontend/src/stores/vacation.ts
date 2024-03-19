import { endOfMonth, startOfMonth } from "date-fns";
import { get, writable } from "svelte/store";
import type { RequestVacation, Vacation, VacationStatus } from "../lib/types";
import { handleGettingVacations } from "../lib/vacation";

export const vacations = writable<Vacation[]>([]);
export const vacationStatusChanged = writable<boolean>(false);
export const datesChanged = writable<boolean>(false);

export const hiddenVacations = writable<string[]>([]);
export function toggleHideVacation(calendarId: string) {
  hiddenVacations.update((hidden) =>
    hidden.includes(calendarId)
      ? hidden.filter((id) => id !== calendarId)
      : [...hidden, calendarId]
  );
}

export const creating = writable<{
  creating: boolean;
  newVacation: RequestVacation;
  isShared: boolean;
}>({
  creating: false,
  newVacation: { start: new Date(), end: new Date() },
  isShared: false,
});

export function startRequestingVacation(startDate?: Date) {
  editing.update((e) => ({ ...e, editing: false }));

  const date = startDate ?? new Date();

  creating.set({
    creating: true,
    newVacation: defaultRequestVacation(date),
    isShared: false,
  });
}

export function stopRequestingVacation() {
  creating.update((c) => ({ ...c, creating: false }));
}

function defaultRequestVacation(date: Date): RequestVacation {
  return {
    start: date,
    end: date,
  };
}

export const editing = writable<{
  editing: boolean;
  newPeriod: { start: Date; end: Date };
  vacationId: string | undefined;
  status: VacationStatus;
}>({
  editing: false,
  newPeriod: { start: new Date(), end: new Date() },
  vacationId: undefined,
  status: "Pending",
});

export function stopEditingVacation() {
  editing.update((c) => ({ ...c, editing: false }));
  vacationStatusChanged.set(false);
  datesChanged.set(false);
}

export function startEditingVacation(id: string) {
  creating.update((e) => ({ ...e, creating: false }));
  let editingVacation = get(vacations).find((vacation) => vacation.id === id);
  if (editingVacation) {
    editing.set({
      vacationId: editingVacation.id,
      newPeriod: { start: editingVacation.start, end: editingVacation.end },
      editing: true,
      status: editingVacation.status,
    });
  }
}

export const currentDate = writable<Date>(new Date());

export function updateVacationInStore(updatedVacation: Vacation) {
  vacations.update((vacations) =>
    vacations.map((e) => (e.id === updatedVacation.id ? updatedVacation : e))
  );
}

export function deleteVacationFromStore(id: string) {
  vacations.update((vacations) => vacations.filter((e) => id != e.id));
}

export function addVacationToStore(vacation: Vacation) {
  vacations.update((vacations) => [...vacations, vacation]);
}

export async function updateVacations(month: Date) {
  const result = await handleGettingVacations({
    start: startOfMonth(month),
    end: endOfMonth(month),
  });
  if (result) {
    vacations.set(result.sort((a, b) => Number(a.start) - Number(b.start)));
  }
}
