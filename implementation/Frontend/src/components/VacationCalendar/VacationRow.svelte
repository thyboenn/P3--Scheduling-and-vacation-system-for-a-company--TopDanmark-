<script lang="ts">
  import { intervalToDuration, startOfMonth } from "date-fns";
  import type { Vacation } from "../../lib/types";
  import { currentDate } from "../../stores/vacation";
  import VacationElement from "./VacationElement.svelte";
  import VacationSpacer from "./VacationSpacer.svelte";

  export let vacations: Vacation[];
  export let name: string;
  export let color: string | null = null;

  function calculateColspan(interval: { start: Date; end: Date }) {
    return (
      intervalToDuration({ start: interval.start, end: interval.end }).days ?? 0 - 1
    );
  }
</script>

<td class="name-list-column">
  <span class="row-height">{name}</span>
</td>
{#each vacations as vacation, i}
  {#if i == 0}
    <VacationSpacer
      colspan={calculateColspan({
        start: startOfMonth($currentDate),
        end: vacation.start,
      })}
    />
    <VacationElement {vacation} {color} />
  {:else}
    <VacationSpacer
      colspan={calculateColspan({
        start: vacation.start,
        end: vacations[i - 1].end,
      }) - 1}
    />
    <VacationElement {vacation} {color} />
  {/if}
{/each}

<style lang="scss">
  @use "../../variables.scss" as variables;

  td {
    padding: 1em;
  }
  table {
    border-collapse: collapse;
  }
  td {
    border-right: variables.$border-style;
  }
</style>
