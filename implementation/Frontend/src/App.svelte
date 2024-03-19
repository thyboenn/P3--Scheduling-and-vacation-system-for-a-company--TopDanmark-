<script lang="ts">
  import "attractions";
  import { Loading } from "attractions";
  import Router from "svelte-spa-router";
  import { isAuthenticated } from "./lib/auth";
  import { routes } from "./lib/routes";
  import { ensureMe, me } from "./stores/associate";

  if (isAuthenticated()) {
    ensureMe();
  }
</script>

{#if isAuthenticated() && !$me}
  <Loading />
{:else}
  <Router {routes} />
{/if}
