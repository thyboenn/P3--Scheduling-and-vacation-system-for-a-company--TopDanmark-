<script lang="ts">
  import { HomeIcon, PhoneIcon } from "svelte-feather-icons";
  import { convertEventToTimeString, dateFormatter } from "../../lib/calendar";
  import type { WorkEvent } from "../../lib/types";

  export let event: WorkEvent;
</script>

<div class="rbc-event-label">
  {`${convertEventToTimeString(event)}`}
</div>
<div class="rbc-event-label">
  {dateFormatter(event.start)}
</div>
<div class="rbc-event-content">
  {#if event.note != null}
    <p>{event.note}</p>
  {/if}
  <div>
    {#if event.workingFromHome}
      <HomeIcon />
    {/if}
    {#if event.atThePhone}
      <PhoneIcon />
    {/if}
  </div>
</div>

<style>
  .rbc-event-label {
    display: block;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
  .rbc-event-label {
    font-size: 80%;
  }
</style>
