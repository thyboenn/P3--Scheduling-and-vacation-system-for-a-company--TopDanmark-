<script lang="ts">
  import { HomeIcon, PhoneIcon } from "svelte-feather-icons";
  import type { WorkEvent } from "../../lib/types";

  export let event: WorkEvent;
</script>

<div>
  {#if event.workingFromHome}
    <HomeIcon />
  {/if}
  {#if event.atThePhone}
    <PhoneIcon />
  {/if}
</div>
