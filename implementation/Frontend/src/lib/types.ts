import type { components } from "../generated/schema";

type DTOType<T> = T extends keyof components["schemas"]
  ? Required<components["schemas"][T]>
  : never;

export type GetRangeDTO = DTOType<"GetRangeDTO">;

export type WorkEventDTO = DTOType<"WorkEventDTO">;
export type CreateWorkEventDTO = DTOType<"CreateWorkEventDTO">;

export type VacationDTO = DTOType<"VacationDTO">;
export type RequestVacationDTO = DTOType<"RequestVacationDTO">;

export type AssociateDTO = DTOType<"AssociateDTO">;
export type MeDTO = DTOType<"MeDTO">;
export type RoleDTO = DTOType<"RoleDTO">;
export type CreateRoleDTO = DTOType<"CreateRoleDTO">;
export type PermissionType = components["schemas"]["PermissionType"];

export type WorkEvent = UseDateInsteadOfString<WorkEventDTO, "start" | "end">;
export type CreateWorkEvent = UseDateInsteadOfString<
  CreateWorkEventDTO,
  "start" | "end"
>;
export type Vacation = UseDateInsteadOfString<VacationDTO, "start" | "end">;
export type RequestVacation = UseDateInsteadOfString<
  RequestVacationDTO,
  "start" | "end"
>;
export type VacationStatus = DTOType<"VacationStatus">;

export type UseDateInsteadOfString<T, F extends keyof T> = Omit<T, F> & {
  [Key in F]: T[Key] extends string ? Date : never;
};

export type UseStringInsteadOfDate<T, F extends keyof T> = Omit<T, F> & {
  [Key in F]: T[Key] extends Date ? string : never;
};

type KeysWith<TObj, ValueType> = {
  [Key in keyof TObj]: TObj[Key] extends ValueType ? Key : never;
}[keyof TObj];

export function convertDatesToStrings<T, F extends KeysWith<T, Date>>(
  object: T,
  array: F[]
): UseStringInsteadOfDate<T, F> {
  return {
    ...object,
    ...Object.fromEntries(array.map((f) => [f, (object[f] as Date).toISOString()])),
  } as UseStringInsteadOfDate<T, F>;
}

export function convertStringsToDates<T, F extends KeysWith<T, string>>(
  object: T,
  array: F[]
): UseDateInsteadOfString<T, F> {
  return {
    ...object,
    ...Object.fromEntries(array.map((f) => [f, new Date(object[f] as string)])),
  } as UseDateInsteadOfString<T, F>;
}
