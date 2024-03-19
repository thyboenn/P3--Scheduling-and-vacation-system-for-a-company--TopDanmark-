import { push } from "svelte-spa-router";
import type { Route } from "./routes";
import type { MeDTO, PermissionType } from "./types";

export const permissionTranslations: { [type in PermissionType]: string } = {
  Admin: "Admin",
  CanEditOthersVacations: "Kan redigere andres feriedage",
  CanApproveOrDenyVacation: "Kan godkende eller afvise ferie",
  CanManageOtherAssociateSchedule: "Kan administrere andres kalendre",
  CanManageOwnSchedule: "Kan administrere egen kalender",
  CanManageSharedSchedule: "Kan administrere fælles kalenderen",
  CanManageAssociates: "Kan administrere medarbejdere",
  CanSeeSchedules: "Kan se kalendre",
  CanEditOwnVacations: "Kan redigere egne feriedage",
  CanSealVacation: "Kan fastlægge feriedage",
  CanSeeVacations: "Kan se feriedage",
  CanManageForcedVacation: "Kan administrere tvungne feriedage",
};

export function hasOneOfPermissions(
  me: MeDTO | undefined,
  permissions: PermissionType[]
): boolean {
  if (!me) return false;

  const associatePermissions = new Set(me.permissions);
  return (
    permissions.some((permission) => associatePermissions.has(permission)) ||
    associatePermissions.has("Admin")
  );
}

export async function redirectWithoutPermission(
  me: MeDTO | undefined,
  permissions: PermissionType[]
) {
  if (me && !hasOneOfPermissions(me, permissions)) {
    const route: Route = "/no-permission";
    await push(route);
  }
}
