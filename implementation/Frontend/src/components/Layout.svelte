<script lang="ts">
  import { Button, Loading } from "attractions";
  import { onMount } from "svelte";
  import {
    CalendarIcon,
    CircleIcon,
    LogOutIcon,
    MenuIcon,
    SunIcon,
    UserIcon,
    UsersIcon,
  } from "svelte-feather-icons";
  import { push } from "svelte-spa-router";
  import { handleLogout } from "../lib/auth";
  import { hasOneOfPermissions } from "../lib/permissions";
  import { isCurrent as isCurrentRoute, type Route } from "../lib/routes";
  import type { PermissionType } from "../lib/types";
  import { ensureMe, me } from "../stores/associate";

  export let extraHeight: string = "0";

  onMount(() => {
    ensureMe();
  });

  let sidebarExpanded = false;

  type IconComponent = typeof CalendarIcon;

  const navigationElements: {
    link: Route;
    title: string;
    icon?: IconComponent;
    requiredPermissions?: PermissionType[];
  }[] = [
    {
      link: "/calendar",
      title: "Kalender",
      icon: CalendarIcon,
      requiredPermissions: ["CanSeeSchedules"],
    },
    {
      link: "/vacation",
      title: "Ferie",
      icon: SunIcon,
      requiredPermissions: ["CanSeeVacations"],
    },
    {
      link: "/associates",
      title: "Ansatte",
      icon: UsersIcon,
      requiredPermissions: ["CanManageAssociates"],
    },
    {
      link: "/roles",
      title: "Roller",
      icon: UserIcon,
      requiredPermissions: ["CanManageAssociates"],
    },
  ];

  function toggleSidebar(): void {
    sidebarExpanded = !sidebarExpanded;
  }
</script>

<div class="main-container">
  <div class="topbar-container-div">
    <div class="topbar-column">
      <div class="topbar-flex">
        <div style:visibility={$$slots.sidebar ? "visible" : "hidden"}>
          <Button on:click={toggleSidebar} round><MenuIcon /></Button>
        </div>
        <div class="topbar-div">
          {#if $me}
            {#each navigationElements as element}
              {#if !element.requiredPermissions || hasOneOfPermissions($me, element.requiredPermissions)}
                <Button
                  on:click={() => push(element.link)}
                  selected={false}
                  disabled={isCurrentRoute(element.link)}
                >
                  <div class="flex-column horizontal-align-center">
                    {#if element.icon}
                      <svelte:component this={element.icon} />
                    {:else}
                      <CircleIcon />
                    {/if}

                    <span>{element.title}</span>
                  </div>
                </Button>
              {/if}
            {/each}
          {:else}
            <Loading />
          {/if}
        </div>
        <Button on:click={handleLogout} danger><LogOutIcon /></Button>
      </div>

      <div class="custom-topbar" style:height={extraHeight}>
        <slot name="custom-topbar" />
      </div>
    </div>
  </div>
  <div class="content-div">
    {#if $$slots.sidebar}
      <div
        class="sidebar-div"
        style:margin-top={extraHeight}
        class:hide-sidebar={!sidebarExpanded}
        class:show-sidebar={sidebarExpanded}
      >
        <div class="margined">
          <slot name="sidebar" />
        </div>
      </div>
    {/if}
    <div class="view-div" style:margin-top={extraHeight}>
      <!--The component specified in the routes list will be shown here-->
      <slot name="content" />
    </div>
  </div>
</div>

<style lang="scss">
  @use "../variables.scss" as variables;

  :root {
    $sidebar-width: 20cm;
  }

  .main-container {
    display: flex;
    justify-content: center;
    flex-direction: column;
  }
  .topbar-div {
    display: flex;
    overflow: scroll;
    justify-content: space-between;
    margin: 0.5em;
  }
  .topbar-container-div {
    position: fixed;
    z-index: 3;
    top: 0;
    width: 100%;
    min-height: variables.$topbar-height;
    background-color: variables.$topbar-color;
    border-bottom: variables.$border-style;
  }
  .topbar-flex {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 1em;
    height: variables.$topbar-height;
  }
  .topbar-column {
    display: flex;
    flex-direction: column;
    height: 100%;
  }
  .content-div {
    padding-top: variables.$topbar-height;
    width: 100%;
  }
  .view-div {
    display: flex;
    flex-direction: column;
    width: 100%;
  }

  .hide-sidebar {
    left: calc(-1 * variables.$sidebar-width);
  }
  .show-sidebar {
    left: 0;
  }
  .sidebar-div {
    position: fixed;
    top: variables.$topbar-height;
    height: 100%;
    width: variables.$sidebar-width;
    z-index: 2;
    display: flex;
    flex-direction: column;
    background-color: variables.$topbar-color;
    border-right: variables.$border-style;
    transition: left 0.25s cubic-bezier(0.82, 0.085, 0.395, 0.895);
    overflow: hidden;
  }
  .margined {
    margin: 0.5cm;
  }
</style>
