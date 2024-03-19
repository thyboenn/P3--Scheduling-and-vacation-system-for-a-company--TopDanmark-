import { get, writable } from "svelte/store";
import { isAuthenticated } from "../lib/auth";
import { makeBackendRequestJson } from "../lib/requestHelpers";
import type { AssociateDTO, MeDTO } from "../lib/types";

export const me = writable<MeDTO | undefined>(undefined);
export const associates = writable<AssociateDTO[]>([]);

export async function ensureMe() {
  if (isAuthenticated() && !get(me)) {
    await updateMe();
  }
}

export async function updateMe() {
  const result = await makeBackendRequestJson("/Associate/me", "get", undefined);
  if (result) {
    me.set(result);
  }
}

export async function ensureAssociates() {
  if (isAuthenticated() && get(associates).length === 0) {
    await updateAssociates();
  }
}

export async function updateAssociates() {
  const result = await makeBackendRequestJson("/Associate", "get", undefined);
  if (result) {
    associates.set(result);
  }
}

export type CustomOption = { name: string; details?: string; id: string };

export function associateToOption(
  associate: Pick<AssociateDTO, "name" | "email" | "id">
): CustomOption {
  return {
    name: associate.name,
    details: associate.email,
    id: associate.id,
  };
}

export const getAssociateOptions: (
  q: string
) => AsyncGenerator<CustomOption[], void, void> = async function* (text: string) {
  const options =
    get(associates)
      ?.filter((associate) =>
        associate.name.toLowerCase().includes(text.toLowerCase())
      )
      .map(associateToOption) ?? [];

  yield options;
};
