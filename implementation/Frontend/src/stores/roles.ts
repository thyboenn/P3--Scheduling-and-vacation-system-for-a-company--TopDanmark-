import { get, writable } from "svelte/store";
import { isAuthenticated } from "../lib/auth";
import { makeBackendRequestJson } from "../lib/requestHelpers";
import type { RoleDTO } from "../lib/types";

export const roles = writable<RoleDTO[] | undefined>(undefined);

export async function ensureRoles() {
  if (isAuthenticated() && !get(roles)) {
    await updateRoles();
  }
}

export async function updateRoles() {
  const result = await makeBackendRequestJson("/Role", "get", undefined);
  if (result) {
    roles.set(result);
  }
}

export function updateRoleInStore(newRole: RoleDTO) {
  roles.update((roles) =>
    roles
      ? roles.map((role) => (role.id === newRole.id ? newRole : role))
      : undefined
  );
}
