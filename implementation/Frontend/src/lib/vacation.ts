import { get } from "svelte/store";
import { me } from "../stores/associate";
import { deleteVacationFromStore } from "../stores/vacation";
import { makeBackendRequest, makeBackendRequestJson } from "./requestHelpers";
import {
  convertDatesToStrings,
  convertStringsToDates,
  type Vacation,
  type VacationDTO,
  type VacationStatus,
} from "./types";

export const statusTranslations: { [type in VacationStatus]: string } = {
  Approved: "Godkendt",
  Denied: "Afvist",
  Pending: "Afventer svar",
};

export async function handleRequestingVacation(requestDuration: {
  start: Date;
  end: Date;
}): Promise<VacationDTO | null> {
  const meValue = get(me);
  if (!meValue) return null;

  const result = await makeBackendRequestJson("/Vacation/request", "post", {
    associateId: meValue.id,
    ...convertDatesToStrings(requestDuration, ["start", "end"]),
  });
  return result;
}

export async function handleDeletingVacation(vacationId: string) {
  const result = await makeBackendRequest("/Vacation/delete", "delete", {
    vacationId: vacationId,
  });
  if (result) {
    deleteVacationFromStore(vacationId);
  }
  return result;
}

export async function handleSettingStatus(
  vacationId: string,
  status: VacationStatus
) {
  const result = await makeBackendRequestJson("/Vacation/set-status", "post", {
    vacationId: vacationId,
    vacationStatus: status,
  });
  return result;
}

export async function handleEditingVacation(
  editedVacationId: string,
  newPeriod: { start: Date; end: Date }
) {
  const result = await makeBackendRequestJson("/Vacation/edit", "post", {
    id: editedVacationId,
    ...convertDatesToStrings(newPeriod, ["start", "end"]),
  });
  return result;
}

export async function handleGettingVacations(requestPeriod: {
  start: Date;
  end: Date;
}): Promise<Vacation[] | undefined> {
  const result = await makeBackendRequestJson(
    "/Vacation/get",
    "post",
    convertDatesToStrings(requestPeriod, ["start", "end"])
  );
  return result?.map((e) => convertStringsToDates(e, ["start", "end"]));
}
