import decodeJWT from "jwt-decode";
import type { components } from "src/generated/schema";
import { makeBackendRequestJson } from "./requestHelpers";
import { redirectIfNotAuthenticated } from "./routes";

const ACCESS_TOKEN_NAME = "accessToken";
const REFRESH_TOKEN_NAME = "refreshToken";

export const setAccessToken = (accessToken: string) =>
  sessionStorage.setItem(ACCESS_TOKEN_NAME, accessToken);
export const setRefreshToken = (refreshToken: string) =>
  sessionStorage.setItem(REFRESH_TOKEN_NAME, refreshToken);

export const getAccessToken = () => sessionStorage.getItem(ACCESS_TOKEN_NAME);
export const getRefreshToken = () => sessionStorage.getItem(REFRESH_TOKEN_NAME);

export const clearAccessToken = () => sessionStorage.setItem(ACCESS_TOKEN_NAME, "");
export const clearRefreshToken = () =>
  sessionStorage.setItem(REFRESH_TOKEN_NAME, "");

export async function handleLogin(
  email: string,
  password: string
): Promise<boolean> {
  const authResponse = await makeBackendRequestJson("/Auth/login", "post", {
    email,
    password,
  });

  return handleAuthResponse(authResponse);
}

export async function handleRegister(
  name: string,
  email: string,
  password: string
): Promise<boolean> {
  const authResponse = await makeBackendRequestJson("/Auth/register", "post", {
    name,
    email,
    password,
  });

  return handleAuthResponse(authResponse);
}

export function handleLogout() {
  clearAccessToken();
  clearRefreshToken();

  redirectIfNotAuthenticated();
}

// Adopted from: https://hasura.io/blog/best-practices-of-using-jwt-with-graphql
export async function handleRefresh(): Promise<boolean> {
  const refreshTokenString = getRefreshToken();
  if (!refreshTokenString) {
    return false;
  }
  const refreshToken = decodeJWT(refreshTokenString);

  const fingerprintHash: string = (refreshToken as any)["fingerprint"];

  const authResponse = await makeBackendRequestJson("/Auth/refresh", "post", {
    refreshToken: refreshTokenString,
    fingerprintHash,
  });

  return handleAuthResponse(authResponse);
}

export function isAuthenticated(): boolean {
  return Boolean(getAccessToken()) && Boolean(getRefreshToken());
}

function handleAuthResponse(
  authResponse: components["schemas"]["AuthResponseDTO"] | null
): boolean {
  if (authResponse && authResponse.accessToken && authResponse.refreshToken) {
    setAccessToken(authResponse.accessToken);
    setRefreshToken(authResponse.refreshToken);

    return true;
  }
  clearAccessToken();
  clearRefreshToken();

  return false;
}
