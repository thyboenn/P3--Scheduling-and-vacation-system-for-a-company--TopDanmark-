import { push } from "svelte-spa-router";
import Associates from "../routes/Associates.svelte";
import Calendar from "../routes/Calendar.svelte";
import Login from "../routes/Login.svelte";
import NoPermission from "../routes/NoPermission.svelte";
import NotFound from "../routes/NotFound.svelte";
import Register from "../routes/Register.svelte";
import Roles from "../routes/Roles.svelte";
import Vacation from "../routes/Vacation.svelte";
import { isAuthenticated } from "./auth";

//URL Example: localhost:5173/#/calendar
export const routes = {
  "/calendar": Calendar,
  "/no-permission": NoPermission,
  "/vacation": Vacation,
  "/associates": Associates,
  "/": Login,
  "/login": Login,
  "/register": Register,
  "/roles": Roles,
  "*": NotFound,
};

export type Route = Exclude<keyof typeof routes, "*">;

export async function redirectIfNotAuthenticated() {
  if (!isAuthenticated()) {
    const route: Route = "/login";
    await push(route);
  }
}

export async function redirectIfAuthenticated() {
  if (isAuthenticated()) {
    const route: Route = "/calendar";
    await push(route);
  }
}

export function isCurrent(route: Route): boolean {
  return window.location.hash === `#${route}`;
}
