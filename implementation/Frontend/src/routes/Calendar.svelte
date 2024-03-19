<script lang="ts">
  import { Button, Checkbox } from "attractions";
  import { endOfWeek, startOfWeek } from "date-fns";
  import { onMount } from "svelte";
  import { PlusIcon } from "svelte-feather-icons";
  import CalendarContent from "../components/Calendar/CalendarContent.svelte";
  import CreateEventCard from "../components/Calendar/CreateEventCard.svelte";
  import EditEventCard from "../components/Calendar/EditEventCard.svelte";
  import Gutter from "../components/Calendar/Gutter.svelte";
  import TimeHeader from "../components/Calendar/TimeHeader.svelte";
  import Toolbar from "../components/Calendar/Toolbar.svelte";
  import Layout from "../components/Layout.svelte";
  import { handleGettingEvents } from "../lib/calendar";
  import {
    hasOneOfPermissions,
    redirectWithoutPermission,
  } from "../lib/permissions";
  import { handleGettingVacations } from "../lib/vacation";
  import {
    associates,
    ensureAssociates,
    me,
    updateAssociates,
  } from "../stores/associate";
  import {
    creating,
    currentDate,
    editing,
    events,
    hiddenCalendars,
    startCreatingEvent,
    stopEditing,
    toggleHideCalendar,
    vacationsInEventRange,
  } from "../stores/calendar";

  redirectWithoutPermission($me, ["CanSeeSchedules"]);

  onMount(() => {
    ensureAssociates();
  });

  currentDate.subscribe(async (newDate) => {
    updateAssociates();
    const requestPeriod = {
      start: startOfWeek(newDate, { weekStartsOn: 1 }),
      end: endOfWeek(newDate, { weekStartsOn: 1 }),
    };
    const [eventsResult, vacationsResult] = await Promise.all([
      handleGettingEvents(requestPeriod),
      handleGettingVacations(requestPeriod),
    ]);

    if (eventsResult) {
      events.set(eventsResult);
    }
    if (vacationsResult) {
      vacationsInEventRange.set(vacationsResult);
    }
  });
</script>

<Layout extraHeight={"1.5cm"}>
  <div class="flex-column outer-div" slot="content">
    <div class="flex-row">
      <div class="flex-column gutter-column">
        <div class="empty-gutter-corner" />
        <div class="gutter-div">
          <Gutter />
        </div>
      </div>

      <div class="flex-column content-column">
        <div class="time-header-div">
          <TimeHeader />
        </div>
        <div class="content-div">
          <CalendarContent />
        </div>
      </div>
    </div>
  </div>
  <svelte:component this={Toolbar} slot="custom-topbar" />
  <div slot="sidebar">
    <Button
      disabled={!hasOneOfPermissions($me, [
        "CanManageOwnSchedule",
        "CanManageSharedSchedule",
        "CanManageOtherAssociateSchedule",
      ])}
      on:click={() => startCreatingEvent()}
    >
      <PlusIcon />
      <span class="left-margin">Opret begivenhed</span>
    </Button>
    <h3>Kalendere</h3>
    <div class="spaced">
      <Checkbox
        title={"Fælles"}
        checked={!$hiddenCalendars.includes("shared")}
        on:change={() => toggleHideCalendar("shared")}
      >
        <span class="ml">{"Fælles"}</span>
      </Checkbox>
      {#each $associates as associate}
        {#if associate.associateScheduleId}
          <Checkbox
            selectorStyle="background-color: {associate.color}; border-color: {associate.color};"
            title={associate.name}
            checked={!$hiddenCalendars.includes(associate.associateScheduleId)}
            on:change={() =>
              associate.associateScheduleId &&
              toggleHideCalendar(associate.associateScheduleId)}
          >
            <span class="ml">{associate.name}</span>
          </Checkbox>
        {/if}
      {/each}
    </div>
  </div>
</Layout>

<EditEventCard show={$editing.editing} on:stopEditing={stopEditing} />

<CreateEventCard show={$creating.creating} />

<style lang="scss">
  @use "../variables.scss" as variables;

  .flex-row {
    display: flex;
    flex-direction: row;
  }
  .flex-column {
    display: flex;
    flex-direction: column;
  }

  .outer-div {
    user-select: none;
  }
  .empty-gutter-corner {
    min-height: variables.$time-header-height;
    border-bottom: variables.$border-style;
  }
  .gutter-div {
    overflow: hidden;
  }
  .time-header-div {
    width: 100%;
    min-height: variables.$time-header-height;
  }
  .content-column {
    flex: 1;
  }
</style>
