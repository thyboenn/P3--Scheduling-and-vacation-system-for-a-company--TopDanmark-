<script lang="ts">
  import { Button, Checkbox } from "attractions";
  import { addMonths, subMonths } from "date-fns";
  import { onMount } from "svelte";
  import {
    ChevronLeftIcon,
    ChevronRightIcon,
    PlusIcon,
  } from "svelte-feather-icons";
  import Layout from "../components/Layout.svelte";
  import CreateVacationCard from "../components/VacationCalendar/CreateVacationCard.svelte";
  import EditVacationCard from "../components/VacationCalendar/EditVacationCard.svelte";
  import VacationCalendar from "../components/VacationCalendar/VacationCalendar.svelte";
  import { monthFormatter } from "../lib/calendar";
  import {
    hasOneOfPermissions,
    redirectWithoutPermission,
  } from "../lib/permissions";
  import { associates, me } from "../stores/associate";
  import {
    creating,
    currentDate,
    editing,
    hiddenVacations,
    startRequestingVacation,
    toggleHideVacation,
  } from "../stores/vacation";

  onMount(() => {
    redirectWithoutPermission($me, ["CanSeeVacations"]);
  });
</script>

<Layout extraHeight="2.5cm">
  <svelte:fragment slot="content">
    <VacationCalendar />
  </svelte:fragment>
  <svelte:fragment slot="custom-topbar">
    <div class="row space-between full-height">
      <Button round on:click={() => ($currentDate = subMonths($currentDate, 1))}>
        <ChevronLeftIcon />
      </Button>
      <div class="flex-column align-center">
        <h2 class="month-heading">{monthFormatter($currentDate)}</h2>
        <h3 class="year-heading">{$currentDate.getFullYear()}</h3>
      </div>
      <Button on:click={() => ($currentDate = addMonths($currentDate, 1))}>
        <ChevronRightIcon />
      </Button>
    </div>
  </svelte:fragment>
  <svelte:fragment slot="sidebar">
    <Button
      disabled={!hasOneOfPermissions($me, ["CanEditOwnVacations"])}
      on:click={() => startRequestingVacation(new Date())}
    >
      <PlusIcon />
      <span class="left-margin">Opret ferie</span>
    </Button>
    <h3>Kalendere</h3>
    <div class="spaced">
      <Checkbox
        title={"Fælles"}
        checked={!$hiddenVacations.includes("shared")}
        on:change={() => toggleHideVacation("shared")}
      >
        <span class="ml">{"Fælles"}</span>
      </Checkbox>
      {#each $associates as associate}
        {#if associate.associateScheduleId}
          <Checkbox
            selectorStyle="background-color: {associate.color}; border-color: {associate.color};"
            title={associate.name}
            checked={!$hiddenVacations.includes(associate.associateScheduleId)}
            on:change={() =>
              associate.associateScheduleId &&
              toggleHideVacation(associate.associateScheduleId)}
          >
            <span class="ml">{associate.name}</span>
          </Checkbox>
        {/if}
      {/each}
    </div>
  </svelte:fragment>
</Layout>

<CreateVacationCard title={"Opret ferie"} show={$creating.creating} />
<EditVacationCard title={"Rediger ferie"} show={$editing.editing} />

<style>
  .month-heading {
    font-size: 2em;
    margin-bottom: 0.2em;
    margin-top: 0;
    text-transform: capitalize;
  }

  .year-heading {
    font-size: 1.5em;
    margin-top: 0;
    margin-bottom: 0;
  }

  .align-center {
    align-items: center;
    justify-content: center;
  }

  .full-height {
    height: 100%;
  }
</style>
