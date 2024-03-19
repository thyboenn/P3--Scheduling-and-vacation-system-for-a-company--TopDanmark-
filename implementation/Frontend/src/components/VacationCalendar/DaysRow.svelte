<script>
  import { Button } from "attractions";
  import { addDays, getDaysInMonth, isSameDay, startOfMonth } from "date-fns";
  import { currentDate, startRequestingVacation } from "../../stores/vacation";

  $: calendarCellsQty = getDaysInMonth($currentDate);
</script>

{#each Array(calendarCellsQty) as _, i}
  <td>
    <Button
      small
      selected={isSameDay(addDays(startOfMonth($currentDate), i), new Date())}
      on:click={() => {
        startRequestingVacation(addDays(startOfMonth($currentDate), i));
      }}
    >
      <span>{i + 1}</span>
    </Button>
  </td>
{/each}
