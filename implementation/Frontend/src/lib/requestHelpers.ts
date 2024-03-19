import type { paths } from "../generated/schema";
import { handleLogout, handleRefresh, isAuthenticated } from "./auth";
import { redirectIfNotAuthenticated } from "./routes";

type ResponsesObject<T> = {
  responses: {
    200: {
      content: {
        "application/json": T;
      };
    };
  };
};

type RequestBodyObject<T> = {
  requestBody?: {
    content: {
      "application/json": T;
    };
  };
};

export type BackendRoute = keyof paths;

type ReturnHelper<T> = T extends (infer E)[] ? ReturnHelper<E>[] : Required<T>;
export async function makeBackendRequest<
  TPath extends keyof paths,
  TMethod extends keyof paths[TPath] & string,
  InputType extends paths[TPath][TMethod] extends RequestBodyObject<infer Input>
    ? Required<Input>
    : undefined
>(
  path: TPath,
  method: TMethod,
  requestBody: InputType,
  retry?: boolean
): Promise<Response | null> {
  const accessToken = sessionStorage.getItem("accessToken");

  const headers: HeadersInit = { "Content-Type": "application/json" };

  if (accessToken) headers["Authorization"] = `Bearer ${accessToken}`;

  const response = await fetch("/api" + path, {
    method: method,
    body: requestBody && JSON.stringify(requestBody),
    headers,
  });

  if (response.status == 401) {
    if ((retry ?? true) && isAuthenticated() && (await handleRefresh())) {
      return makeBackendRequest(path, method, requestBody, false);
    } else {
      handleLogout();
      redirectIfNotAuthenticated();
      return null;
    }
  }

  return response;
}

export async function makeBackendRequestJson<
  TPath extends keyof paths,
  TMethod extends keyof paths[TPath] & string,
  InputType extends paths[TPath][TMethod] extends RequestBodyObject<infer Input>
    ? Required<Input>
    : undefined,
  ReturnType extends paths[TPath][TMethod] extends ResponsesObject<infer Return>
    ? ReturnHelper<Return>
    : never
>(
  path: TPath,
  method: TMethod,
  requestBody: InputType
): Promise<ReturnType | null> {
  const response = await makeBackendRequest(path, method, requestBody);

  const object = await response?.json();

  if (object == null) return null;

  return object as ReturnType;
}

export function dateToString(year: number, month: number, day: number): string {
  return new Date(year, month - 1, day + 1).toISOString();
}
