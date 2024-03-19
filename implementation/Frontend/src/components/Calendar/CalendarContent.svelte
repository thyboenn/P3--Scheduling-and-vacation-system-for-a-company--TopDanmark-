<script lang="ts">
  import { isSameDay, set } from "date-fns";
  import {
    convertPercentToTime,
    getWeekDays,
    hourStrings,
    layoutEventsForDay,
  } from "../../lib/calendar";
  import { hasOneOfPermissions } from "../../lib/permissions";
  import { associates, me } from "../../stores/associate";
  import {
    currentDate,
    editing,
    events,
    hiddenCalendars,
    startCreatingEvent,
  } from "../../stores/calendar";
  import Event from "./Event.svelte";

  function onMouseDown(e: MouseEvent, day: Date) {
    let clickedElement = e.target as HTMLElement;
    if (
      clickedElement.classList.contains("rbc-events-container") &&
      hasOneOfPermissions($me, [
        "CanManageOwnSchedule",
        "CanManageSharedSchedule",
        "CanManageOtherAssociateSchedule",
      ])
    ) {
      let totalHeight = clickedElement.offsetHeight;
      let mouseHeight = e.offsetY;
      let percentHeight = mouseHeight / totalHeight;
      let startTime = set(convertPercentToTime(percentHeight), {
        year: day.getFullYear(),
        month: day.getMonth(),
        date: day.getDate(),
      });
      startCreatingEvent(startTime);
    }
  }

  $: days = getWeekDays($currentDate).map(
    (day) =>
      [
        day,
        layoutEventsForDay(
          $events.filter(
            (e) =>
              isSameDay(e.start, day) &&
              !$hiddenCalendars.includes(
                $associates.some((a) => a.associateScheduleId === e.scheduleId)
                  ? e.scheduleId
                  : "shared"
              )
          )
        ),
      ] as const
  );
</script>

<div class="days-container">
  {#each days as [day, dayLayout]}
    <div class="rbc-day-slot rbc-time-column">
      {#each hourStrings as _}
        <div class="rbc-timeslot-group" />
      {/each}
      <div class="rbc-events-container" on:mousedown={(e) => onMouseDown(e, day)}>
        {#each dayLayout.events as { event, layout: eventLayout } (event.id)}
          <Event
            {event}
            columnIndex={eventLayout.columnIndex}
            columnSpan={eventLayout.columnSpan}
            columnCount={dayLayout.columnCount}
            selected={$editing.selectedEvent == event && $editing.editing}
          />
        {/each}
      </div>
    </div>
  {/each}
</div>

<style>
  .days-container {
    display: flex;
    flex-direction: row;
  }
  .rbc-day-slot {
    position: relative;
  }
  .rbc-time-column {
    display: flex;
    flex: 1;
    flex-direction: column;
    min-height: 100%;
  }

  .rbc-day-slot .rbc-events-container {
    bottom: 0;
    left: 0;
    position: absolute;
    right: 0;
    margin-right: 10px;
    top: 0;
  }

  /* Custom CSS */
  .rbc-events-container {
    display: inline-block;
  }
  .rbc-timeslot-group {
    display: inline-block;
  }
</style>
